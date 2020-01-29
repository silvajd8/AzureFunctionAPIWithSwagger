using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Net;
using AzureFunctions.Extensions.Swashbuckle.Attribute;

namespace TodoAPI
{
    public static class TodoAPI
    {
        public static IList<Todo> Todos = new List<Todo>();

        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Todo[]))]
        [FunctionName("List")]
        public static IActionResult Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("GET TODO LIST");
            return new OkObjectResult(Todos);
        }

        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Todo))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [FunctionName("Get")]
        public static IActionResult GetById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo/{id}")] HttpRequest req,
            ILogger log,
            int id)
        {
            log.LogInformation("GET TODO BY ID");

            var todo = Todos.FirstOrDefault(e => e.Id == id);

            if (todo == null)
            {
                log.LogInformation($"ID {id} NOT FOUND!");
                return new NotFoundResult();
            }
                

            return new OkObjectResult(todo);
        }

        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Todo))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [FunctionName("Post")]
        public static IActionResult Post(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")]
            [RequestBodyType(typeof(Todo), "product request")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("CREATE A TODO");

            var json = new StreamReader(req.Body).ReadToEnd();
            var todo = JsonConvert.DeserializeObject<Todo>(json);

            if (string.IsNullOrEmpty(todo.Description))
            {
                log.LogInformation($"NO NAME SENDED!");
                return new BadRequestObjectResult("INFORM A DESCRIPTION FOR TODO");
            }

            todo.Id = Todos.Count + 1;
            todo.RegistrationTime = DateTime.UtcNow;
            Todos.Add(todo);

            return new OkObjectResult(todo);
        }

        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Todo))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [FunctionName("Put")]
        public static IActionResult Do(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo/{id}")]
            [RequestBodyType(typeof(Todo), "product request")] HttpRequest req,
            ILogger log,
            int id)
        {
            log.LogInformation("CREATE A TODO");

            var todo = Todos.FirstOrDefault(e => e.Id == id);

            if (todo == null)
            {
                log.LogInformation($"ID {id} NOT FOUND!");
                return new NotFoundResult();
            }

            var json = new StreamReader(req.Body).ReadToEnd();
            var input = JsonConvert.DeserializeObject<Todo>(json);

            todo.Description = input.Description;
            todo.HasDone = input.HasDone;
            todo.RegistrationTime = DateTime.UtcNow;

            return new OkObjectResult(todo);
        }

        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Todo))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [FunctionName("Delete")]
        public static IActionResult Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todo/{id}")] HttpRequest req,
            ILogger log,
            int id)
        {
            log.LogInformation("DELTE A TODO");

            var todo = Todos.FirstOrDefault(e => e.Id == id);

            if (todo == null)
            {
                log.LogInformation($"ID {id} NOT FOUND!");
                return new NotFoundResult();
            }

            var result = Todos.Remove(todo);

            return new OkObjectResult(result);
        }
    }
}
