﻿namespace Fiesta.Application.Common.Constants
{
    public class ErrorCodes
    {
        public const string InvalidEmailAddress = "invalidEmail";
        public const string InvalidCode = "invalidCode";
        public const string Required = "required";
        public const string MinLength = "minLength";
        public const string MaxLength = "maxLength";
        public const string MustBeUnique = "mustBeUnique";
        public const string EmailAreadyInUse = "emailAlreadyInUse";
        public const string InvalidLoginCredentials = "invalidLoginCredentials";
        public const string InvalidAuthProvider = "invalidAuthProvider";
        public const string EmailIsNotVerified = "emailIsNotVerified";
        public const string EmailAlreadyVerified = "emailAlreadyVerified";
        public const string InvalidPassword = "invalidPassword";
        public const string InvalidRefreshToken = "invalidRefreshToken";
        public const string RefreshTokenExpired = "refreshTokenExpired";
        public const string AccountAlreadyConnectedToGoogleWithDifferentEmail = "accounutAlreadyConnectedToGoogleWithDifferentEmail";
        public const string GoogleAccountNotConnected = "thisGoogleAccountIsNotConnected";
        public const string MustBeInTheFuture = "mustBeInTheFuture";
        public const string MustBeAfterStartDate = "mustBeAfterStartDate";
        public const string Max = "max";
        public const string Min = "min";
        public const string Invalid = "invalid";
        public const string InvalidEnumValue = "invalidEnumValue";
        public const string InvalidLatitudeOrLongitude = "invalidLatitudeOrLongitude";
        public const string UnsupportedMediaType = "unsupportedMediaType";
        public const string MaxSize = "maxSize";
        public const string MinSize = "minSize";
        public const string AlreadyExists = "alreadyExists";
        public const string CanContainLettersNumberOrSpecialCharacters = "canContainLettersNumberOrSpecialCharacters";
        public const string DoesNotExist = "doesNotExist";
        public const string AlreadyFriends = "alreadyFriends";
        public const string SenderAndReceiverIdentical = "senderAndReceiverIdentical";
        public const string AlreadyAttendeeOrInvited = "alreadyAttendeeOrInvited";
        public const string InvalidOperation = "invalidOperation";
        public const string EventIsFull = "eventIsFull";
        public const string CannotBeLessThanCurrentAttendeesCount = "cannotBeLessThanCurrentAttendeesCount";
        public const string ServiceUnavailable = "serviceUnavailable";
        public const string EitherExternalLinkOrLocationMustBeSet = "eitherExternalLinkOrLocationMustBeSet";
    }
}
