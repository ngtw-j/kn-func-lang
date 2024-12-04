﻿using CloudNative.CloudEvents.AspNetCore;
using CloudNative.CloudEvents.Http;
using CloudNative.CloudEvents.NewtonsoftJson;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace CloudNative.CloudEvents.AspNetCoreSample.Controllers
{
    [Route("")]
    [ApiController]
    public class CloudEventController(ILogger<CloudEventController> logger) : ControllerBase
    {
        private readonly ILogger<CloudEventController> _logger = logger;
        private static readonly CloudEventFormatter formatter = new JsonEventFormatter(); 

        /// <summary>
        /// Generates a CloudEvent in "structured mode", where all CloudEvent information is
        /// included within the body of the response.
        /// </summary>
        [HttpGet]
        public ActionResult<string> GenerateCloudEvent()
        {
            var evt = new CloudEvent
            {
                Type = "CloudNative.CloudEvents.AspNetCoreSample",
                Source = new Uri("https://github.com/cloudevents/sdk-csharp"),
                Time = DateTimeOffset.Now,
                DataContentType = "application/json",
                Id = Guid.NewGuid().ToString(),
                Data = new
                {
                    Language = "C#",
                    EnvironmentVersion = Environment.Version.ToString()
                }
            };
            // Format the event as the body of the response. This is UTF-8 JSON because of
            // the CloudEventFormatter we're using, but EncodeStructuredModeMessage always
            // returns binary data. We could return the data directly, but for debugging
            // purposes it's useful to have the JSON string.
            var bytes = formatter.EncodeStructuredModeMessage(evt, out var contentType);
            string json = Encoding.UTF8.GetString(bytes.Span);
            var result = Ok(json);

            // Specify the content type of the response: this is what makes it a CloudEvent.
            // (In "binary mode", the content type is the content type of the data, and headers
            // indicate that it's a CloudEvent.)
            result.ContentTypes.Add(contentType.MediaType);
            _logger.LogInformation("Generated CloudEvent");
            return result;
        }

        [HttpPost]
        public async Task<IActionResult> HandleCloudEvent()
        {
            /*
            * YOUR CODE HERE
            *
            */
            _logger.LogInformation("Received POST request");
            CloudEvent incomingCE = await Request.ToCloudEventAsync(formatter);
            _logger.LogInformation($"Incoming CloudEvent: {incomingCE.Id}");
            CloudEvent outgoingCE = incomingCE;
            return Ok(outgoingCE);  
        }

 
    }
}