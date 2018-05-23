using System;

namespace UHtml.Core.Entities
{
    /// <summary>
    /// Raised when an error occurred during html rendering.
    /// </summary>
    public sealed class HtmlRenderErrorEventArgs : EventArgs
    {
        /// <summary>
        /// error type that is reported
        /// </summary>
        private readonly HtmlRenderErrorType type;

        /// <summary>
        /// the error message
        /// </summary>
        private readonly string message;

        /// <summary>
        /// the exception that occurred (can be null)
        /// </summary>
        private readonly Exception exception;

        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="type">the type of error to report</param>
        /// <param name="message">the error message</param>
        /// <param name="exception">optional: the exception that occurred</param>
        public HtmlRenderErrorEventArgs(HtmlRenderErrorType type, string message, Exception exception = null)
        {
            this.type = type;
            this.message = message;
            this.exception = exception;
        }

        /// <summary>
        /// error type that is reported
        /// </summary>
        public HtmlRenderErrorType Type
        {
            get { return type; }
        }

        /// <summary>
        /// the error message
        /// </summary>
        public string Message
        {
            get { return message; }
        }

        /// <summary>
        /// the exception that occurred (can be null)
        /// </summary>
        public Exception Exception
        {
            get { return exception; }
        }

        public override string ToString()
        {
            return string.Format("Type: {0}", type);
        }
    }
}