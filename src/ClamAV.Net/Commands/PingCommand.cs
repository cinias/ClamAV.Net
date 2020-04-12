﻿using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClamAV.Net.Commands.Base;
using ClamAV.Net.Exceptions;

namespace ClamAV.Net.Commands
{
    internal class PingCommand : BaseCommand, ICommand<string>
    {
        private const string EXPECTED_RESPONSE = "PONG";

        public PingCommand() : base("PING")
        {
        }

        public Task<string> ProcessRawResponseAsync(byte[] rawResponse, CancellationToken cancellationToken = default)
        {
            if (rawResponse == null)
                return Task.FromException<string>(new ClamAVException($"Raw response is null"));

            string actualResponse = Encoding.UTF8.GetString(rawResponse);

            return string.Equals(EXPECTED_RESPONSE, actualResponse, StringComparison.Ordinal)
                ? Task.FromResult(actualResponse)
                : Task.FromException<string>(new ClamAVException($"Unexpected response {actualResponse}"));
        }
    }
}