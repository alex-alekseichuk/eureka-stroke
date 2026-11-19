jQuery(function() {
    jQuery.extend(jQuery, { EurekaStroke: function() { } });
    jQuery.extend(jQuery.EurekaStroke, { Uploader: function() { return this; } });
    jQuery.extend(jQuery.EurekaStroke.Uploader, {
        tmplUploadLog: tmpl("tmplUploadLog")
        , gridUploadLog: jQuery('#UploadLog')
        , Items: []
        , Mode: 'existing' // existing, new
        , isUploading: false
        , render: function() {
            this.gridUploadLog.html(this.tmplUploadLog(this));
        }
        , setMode: function(mode) {
            this.Mode = mode;
            jQuery('.divMode').hide();
            jQuery('#divMode_' + this.Mode).show();
        }

        , RefreshTimeout: 60000
        , startTimer: function() {
            var uploader = this;
            uploader.timer = setTimeout(function() {
                uploader.update();
            }, uploader.RefreshTimeout)
        }

        , isNewData: function(items) {
            var uploader = this;
            if (items.length != uploader.Items.length)
                return true;
            var n = uploader.length;
            for (var i = 0; i < n; ++i) {
                var a = uploader.Items[i];
                var b = items[i];
                if (a.Filename != b.FileName)
                    return true;
            }
            return false;
        }
        , update: function(callback) {
            var uploader = this;
            if (uploader.timer) {
                clearTimeout(uploader.timer);
                uploader.timer = 0;
            }

            jQuery.ajax({
                type: "POST",
                url: "UploaderWebService.asmx/GetUploadLog",
                data: "{}",
                timeout: 20000,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                cache: false, // adds ?_=unique_long_integer
                success: function(response) {
                    if (uploader.isNewData(response.d)) {
                        uploader.Items = response.d;
                        uploader.render();
                    }
                    if (typeof callback == 'function')
                        callback();
                },
                complete: function() {
                    uploader.startTimer();
                }
            });

        }

        , updateFacilities: function(callback) {
            var uploader = this;
            jQuery.ajax({
                type: "POST",
                url: "UploaderWebService.asmx/GetFacilities",
                data: "{}",
                timeout: 20000,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                cache: false, // adds ?_=unique_long_integer
                success: function(response) {
                    var options = '';
                    $.each(response.d, function(i, facility) {
                        options += '<option value="' + facility.ID + '">' + facility.Name + '</option>';
                    });
                    $('#selFacility').html(options);

                    if (typeof callback == 'function')
                        callback();
                }
            });
        }

        , upload: function() {
            var uploader = this;
            if (uploader.isUploading)
                return;
            var mode = jQuery(".mode:checked").val();
            if (mode == 'new') {
                var FacilityName = jQuery("#txtFacilityName").val();
                if (!FacilityName) {
                    uploader.showError("Provide some Name for new Facility");
                    return;
                }
                uploader.onBeforeUpload();
                uploader.checkFacilityName(FacilityName, function() {
                    uploader.ajaxFileUpload(function() {
                        uploader.onAfterUpload();
                        uploader.updateFacilities();
                    });
                });
            } else {
                uploader.onBeforeUpload();
                uploader.ajaxFileUpload(function() { uploader.onAfterUpload(); });
            }
        }
        , onBeforeUpload: function() {
            var uploader = this;
            uploader.isUploading = true;
            jQuery('#upload_animation').show();
        }
        , onAfterUpload: function() {
            var uploader = this;
            uploader.isUploading = false;
            jQuery('#upload_animation').hide();
        }
        , checkFacilityName: function(FacilityName, callback) {
            var uploader = this;
            jQuery.ajax({
                type: "POST",
                url: "UploaderWebService.asmx/CheckFacilityName",
                data: '{"FacilityName":"' + FacilityName + '"}',
                timeout: 10000,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                cache: false,
                success: function(response) {
                    if (!response.d) {
                        if (typeof callback == 'function')
                            callback();
                    } else {
                        uploader.onAfterUpload();
                        uploader.showError(response.d);
                    }
                },
                complete: function() {
                }
            });
        }
        , ajaxFileUpload: function(callback) {
            var uploader = this;
            jQuery.ajaxFileUpload
	        (
		        {
		            url: 'UploaderWebService.asmx/Upload',
		            secureuri: false,
		            fileElementId: 'fileSource',
		            dataType: 'xml',
		            params: {
		                "Mode": jQuery(".mode:checked").val(),
		                "FormatID": jQuery("#selFormat").val(),
		                "FacilityID": jQuery("#selFacility").val(),
		                "FacilityName": jQuery("#txtFacilityName").val()
		            }
		            , beforeSend: function() {
		            }
		            , complete: function() {
    		            if (typeof callback == 'function')
		                    callback();
		            }
		            , success: function(data, status) {
		            var result = data.documentElement.firstChild.nodeValue;
		                if ('+' == result.substring(0, 1)) {
		                    uploader.showMessage(result.substring(1));
		                    uploader.update();
		                } else {
		                    uploader.showError(result.substring(1));
		                }
		            }
		            , error: function(data, status, e) {
		                if (window.navigator.appName != "Opera")
		                    uploader.showError(e.message);
		            }
		        }
	        )
            return false;
        }
        , Statuses: {
            0: 'Done',
            1: 'Pending',
            2: 'Error',
            3: 'In progress'
        }

        , showError: function(error) {
            jQuery('#boxMessage').hide();
            jQuery('#boxError')
                .show()
                .html(error);
        }
        , showMessage: function(message) {
            jQuery('#boxError').hide();
            jQuery('#boxMessage')
                .show()
                .html(message);
        }

        , init: function(options) {
            var uploader = this;

            jQuery.extend(uploader, options);
            
            // init mode (existing or new)
            jQuery("input#mode_" + uploader.Mode).attr('checked', 'checked');
            uploader.setMode(uploader.Mode);
            jQuery("input.mode").click(function() {
                var $this = jQuery(this);
                uploader.setMode($this.val());
            });

            // form submit handler
            jQuery('#formUpload').submit(function() {
                uploader.upload();
                return false;
            });
            // upload button handler
            jQuery('#btnUpload').click(function() {
                uploader.upload();
                return false;
            });

            // initial grid render and start timer to update
            uploader.render();
            uploader.startTimer();
        }
    });
});
