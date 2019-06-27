﻿namespace EventNames
{
    public static class ClientToServer
    {
        // Authentication Events
        public const string SubmitRegisterAccount = "SubmitRegisterAccount";
        public const string SubmitLoginAccount = "SubmitLoginAccount";
        public const string SubmitEmailToken = "SubmitEmailToken";
        public const string SubmitGoogleAuthCode = "SubmitGoogleAuthCode";
        public const string SubmitFirstEmailToken = "SubmitFirstEmailToken";
        public const string SubmitNewVerificationEmail = "SubmitNewVerificationEmail";
        public const string SubmitResendEmail = "SubmitResendEmail";
        public const string SubmitBackToLogin = "SubmitBackToLogin";
        public const string SubmitEnableGoogleAuthCode = "SubmitEnableGoogleAuthCode";

        // Other
        public const string SubmitPlayerCommand = "SubmitPlayerCommand";
    }

    public static class ServerToClient
    {
        // Authentication Events
        public const string SetLoginScreen = "SetLoginScreen";
        public const string DisplayError = "DisplayError";
        public const string RegistrationSuccess = "RegistrationSuccess";
        public const string Show2FAbyEmailAddress = "Show2FAbyEmailAddress";
        public const string Show2FAbyGoogleAuth = "Show2FAbyGoogleAuth";
        public const string ShowInitialEmailVerification = "ShowInitialEmailVerification";
        public const string ShowChangeEmailAddress = "ShowChangeEmailAddress";
        public const string ShowLoginPage = "ShowLoginPage";
        public const string ShowQRCode = "ShowQRCode";
        public const string ShowQRCodeEnabled = "ShowQRCodeEnabled";
    }
}
