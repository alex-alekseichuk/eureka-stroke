jQuery.extend({
    createUploadIframe: function(id, uri) {
        //create frame
        var frameId = 'jUploadFrame' + id;

        if (window.ActiveXObject) {
            var io = document.createElement('<iframe id="' + frameId + '" name="' + frameId + '" />');
            if (typeof uri == 'boolean') {
                io.src = 'javascript:false';
            }
            else if (typeof uri == 'string') {
                io.src = uri;
            }
        }
        else {
            var io = document.createElement('iframe');
            io.id = frameId;
            io.name = frameId;
        }
        io.style.position = 'absolute';
        io.style.top = '-1000px';
        io.style.left = '-1000px';

        document.body.appendChild(io);

        return io
    },
    createUploadForm: function(id, fileElementId) {
        //create form
        var formId = 'jUploadForm' + id;
        var fileId = 'jUploadFile' + id;
        var form = jQuery('<form  action="" method="POST" name="' + formId + '" id="' + formId + '" enctype="multipart/form-data"></form>');
        var oldElement = jQuery('#' + fileElementId);
        var newElement = jQuery(oldElement).clone();
        jQuery(oldElement).attr('id', fileId);
        jQuery(oldElement).before(newElement);
        jQuery(oldElement).appendTo(form);
        //set attributes
        jQuery(form).css('position', 'absolute');
        jQuery(form).css('top', '-1200px');
        jQuery(form).css('left', '-1200px');
        jQuery(form).appendTo('body');
        return form;
    },

    ajaxFileUpload: function(s) {
        // TODO introduce global settings, allowing the client to modify them for all requests, not only timeout		
        s = jQuery.extend({}, jQuery.ajaxSettings, s);
        var id = new Date().getTime()
        var form = jQuery.createUploadForm(id, s.fileElementId);
        var io = jQuery.createUploadIframe(id, s.secureuri);
        if (typeof s.params != 'undefined')
            for (key in s.params)
            jQuery(form).append(jQuery('<input type="hidden" name="' + key + '" value="' + s.params[key] + '" />'));
        var frameId = 'jUploadFrame' + id;
        var formId = 'jUploadForm' + id;
        // Watch for a new set of requests
        if (s.global && !jQuery.active++) {
            jQuery.event.trigger("ajaxStart");
        }
        var requestDone = false;
        // Create the request object
        var xmlObject = {};
        if (s.global)
            jQuery.event.trigger("ajaxSend", [xmlObject, s]);
        // Wait for a response to come back
        var uploadCallback = function(isTimeout) {
            var io = document.getElementById(frameId);
            try {
                if (io.contentWindow && io.contentWindow.document) {
                    if (io.contentWindow.document.body)
                        xmlObject.responseText = io.contentWindow.document.body.innerHTML;
                    else
                        xmlObject.responseText = null;
                    if (io.contentWindow.document.XMLDocument)
                        xmlObject.responseXML = io.contentWindow.document.XMLDocument;
                    else
                        xmlObject.responseXML = io.contentWindow.document;
                } else if (io.contentDocument && io.contentDocument.document) {
                    xmlObject.responseText = io.contentDocument.document.body ? io.contentDocument.document.body.innerHTML : null;
                    xmlObject.responseXML = io.contentDocument.document.XMLDocument ? io.contentDocument.document.XMLDocument : io.contentDocument.document;
                }
            } catch (e) {
                //alert(xmlObject);
                jQuery.handleError(s, xmlObject, null, e);
            }
            if (xmlObject || isTimeout == "timeout") {
                requestDone = true;
                var status;
                try {
                    status = isTimeout != "timeout" ? "success" : "error";
                    // Make sure that the request was successful or notmodified
                    if (status != "error") {
                        // process the data (runs the xml through httpData regardless of callback)
                        var data = jQuery.uploadHttpData(xmlObject, s.dataType);
                        // If a local callback was specified, fire it and pass it the data
                        if (s.success)
                            s.success(data, status);

                        // Fire the global callback
                        if (s.global)
                            jQuery.event.trigger("ajaxSuccess", [xmlObject, s]);
                    } else
                        jQuery.handleError(s, xmlObject, status);
                } catch (e) {
                    status = "error";
                    jQuery.handleError(s, xmlObject, status, e);
                }

                // The request was completed
                if (s.global)
                    jQuery.event.trigger("ajaxComplete", [xmlObject, s]);

                // Handle the global AJAX counter
                if (s.global && ! --jQuery.active)
                    jQuery.event.trigger("ajaxStop");

                // Process result
                if (s.complete)
                    s.complete(xmlObject, status);
                //clear up the created form after file uploaded.
                jQuery(io).unbind()

                setTimeout(function() {
                    try {
                        jQuery(io).remove();
                        jQuery(form).remove();
                        jQuery('#' + s.fileElementId).val('');

                    } catch (e) {
                        jQuery.handleError(s, xmlObject, null, e);
                    }

                }, 100)

                xmlObject = null

            }
        }
        // Timeout checker
        if (s.timeout > 0) {
            setTimeout(function() {
                // Check to see if the request is still happening
                if (!requestDone) uploadCallback("timeout");
            }, s.timeout);
        }
        try {
            // var io = jQuery('#' + frameId);
            var form = jQuery('#' + formId);
            jQuery(form).attr('action', s.url);
            jQuery(form).attr('method', 'POST');
            jQuery(form).attr('target', frameId);
            if (form.encoding) {
                form.encoding = 'multipart/form-data';
            }
            else {
                form.enctype = 'multipart/form-data';
            }
            jQuery(form).submit();

        } catch (e) {
            jQuery.handleError(s, xmlObject, null, e);
        }
        if (window.attachEvent) {
            document.getElementById(frameId).attachEvent('onload', uploadCallback);
        }
        else {
            document.getElementById(frameId).addEventListener('load', uploadCallback, false);
        }
        return { abort: function() { } };

    },

    uploadHttpData: function(r, type) {
        var data = !type;
        data = type == "xml" || data ? r.responseXML : r.responseText;
        // If the type is "script", eval it in global context
        if (type == "script")
            jQuery.globalEval(data);
        // Get the JavaScript object, if JSON is used.
        if (type == "json")
            eval("data = " + data);
        // evaluate scripts within html
        if (type == "html")
            jQuery("<div>").html(data).evalScripts();
        //alert(jQuery('param', data).each(function(){alert(jQuery(this).attr('value'));}));
        return data;
    }
});

