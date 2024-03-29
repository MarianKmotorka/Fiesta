﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Options;
using Fiesta.Application.Models.Emails;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using Newtonsoft.Json.Linq;

namespace Fiesta.Infrastracture.Messaging.Email
{
    public class EmailService : IEmailService
    {
        private readonly string _pathToTemplates = $"Fiesta.Infrastracture.Messaging.Email.Templates.";
        private readonly IFluentEmail _fluentEmail;
        private readonly WebClientOptions _webClientOptions;

        public EmailService(IFluentEmail fluentEmail, WebClientOptions webClientOptions)
        {
            _fluentEmail = fluentEmail;
            _webClientOptions = webClientOptions;
        }

        public async Task<SendResponse> SendResetPasswordEmail(string emailAddress, ResetPasswordEmailTemplateModel model, CancellationToken cancellationToken)
        {
            var urlEncodedToken = HttpUtility.UrlEncode(model.Token);
            var redirectUrl = $"{_webClientOptions.BaseUrl}/reset-password?token={urlEncodedToken}&email={emailAddress}";

            var template = BuildEmailUsingTemplate(
                emailAddress,
                EmailSubjects.PasswordReset,
                TemplateNames.ResetPasswordEmail,
                new { RedirectUrl = redirectUrl }
                );

            try
            {
                return await template.SendAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                return new SendResponse { ErrorMessages = new List<string>() { ex.Message } };
            }
        }

        public async Task<SendResponse> SendVerificationEmail(string emailAddress, VerificationEmailTemplateModel model, CancellationToken cancellationToken)
        {
            var urlEncodedCode = HttpUtility.UrlEncode(model.Code);
            var redirectUrl = $"{_webClientOptions.BaseUrl}/confirm-email?code={urlEncodedCode}&email={emailAddress}";

            var template = BuildEmailUsingTemplate(
                emailAddress,
                EmailSubjects.VerificationEmail,
                TemplateNames.VerificationEmail,
                new
                {
                    model.Name,
                    RedirectUrl = redirectUrl
                });

            try
            {
                return await template.SendAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                return new SendResponse { ErrorMessages = new List<string>() { ex.Message } };
            }
        }

        private IFluentEmail BuildEmailUsingTemplate(string emailAddress, string subject, string template, object model)
        {
            var email = _fluentEmail
            .To(emailAddress)
            .Subject(subject)
            .UsingTemplateFromEmbedded(_pathToTemplates + template, JObject.FromObject(model), GetType().Assembly);

            return email;
        }
    }
}
