using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tavis.Http;
using Tavis.HttpResponseMachine;
using Xunit;

namespace LinkTests
{
    public class LoginFormModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string StatusMessage { get; set; }
    }

    public class Controller
    {
        private LoginFormModel _loginFormModel;
        public HttpResponseMachine Machine {get;set;}

        public Controller(LoginFormModel model)
        {
            _loginFormModel = model;
            Machine = new HttpResponseMachine();

            Machine
                .When(HttpStatusCode.OK, linkRelation: "login", contentType: null, profile: null)
                .Then(LoginSuccessful);

            Machine.When(HttpStatusCode.Unauthorized, linkRelation: "login", contentType: null, profile: null)
                .Then(LoginFailed);

            Machine.When(HttpStatusCode.Forbidden, linkRelation: "login", contentType: null, profile: null)
                .Then(LoginForbidden);

            Machine.When(HttpStatusCode.BadRequest, linkRelation: "login", contentType: null, profile: null)
                .Then(FailedRequest);

            Machine.When(HttpStatusCode.OK, linkRelation: "reset", contentType: null, profile: null)
                .Then(ResetForm);

        }

        public async Task LoginSuccessful(IRequestFactory requestFactory, HttpResponseMessage response)
        {
            _loginFormModel.StatusMessage = "Successfully logged in";
            
        }

        public async Task ResetForm(IRequestFactory requestFactory, HttpResponseMessage response)
        {
            _loginFormModel.UserName = "";
            _loginFormModel.Password = "";

        }


        public async Task LoginFailed(IRequestFactory requestFactory, HttpResponseMessage response)
        {
            _loginFormModel.StatusMessage = "Credentials invalid";

        }

        public async Task LoginForbidden(IRequestFactory requestFactory, HttpResponseMessage response)
        {
            _loginFormModel.StatusMessage = "Insufficient Permissions";

        }

        public async Task FailedRequest(IRequestFactory requestFactory, HttpResponseMessage response)
        {
            _loginFormModel.StatusMessage = "Unable to login -  status code " + response.StatusCode;

        }  
    }
    public class ModelMachineControllerTests
    {
        [Fact]
        public async Task LoginSuccessful()
        {
            var loginFormModel = new LoginFormModel();
            var controller = new Controller(loginFormModel);

            controller.Machine.HandleResponseAsync(new FakeLink { LinkRelation = "login" }, new HttpResponseMessage(HttpStatusCode.OK));
            Assert.Equal("Successfully logged in", loginFormModel.StatusMessage);
        
        }

        [Fact]
        public async Task LoginFailed()
        {
            var loginFormModel = new LoginFormModel();
            var controller = new Controller(loginFormModel);

            controller.Machine.HandleResponseAsync(new FakeLink { LinkRelation = "login" }, new HttpResponseMessage(HttpStatusCode.Unauthorized));
            Assert.Equal("Credentials invalid", loginFormModel.StatusMessage);

        }

        [Fact]
        public async Task ResetForm()
        {
            var loginFormModel = new LoginFormModel()
            {
                UserName = "bob",
                Password = "foo"
            };
            var controller = new Controller(loginFormModel);

            controller.Machine.HandleResponseAsync(new FakeLink { LinkRelation = "reset" }, new HttpResponseMessage(HttpStatusCode.OK));
            Assert.Equal("", loginFormModel.UserName);
            Assert.Equal("", loginFormModel.Password);
        }

    }
    public class FakeLink : IRequestFactory
    {
        public string LinkRelation
        {
            get; set;
        }
        public Uri Target
        {
            get; set;
        }
        public HttpRequestMessage CreateRequest()
        {
            return new HttpRequestMessage() { RequestUri = Target};
        }
    }
}
