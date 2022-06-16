using Newtonsoft.Json;

namespace Barriot
{
    public class VotingMiddleware
    {
        private readonly string _authKey;
        private readonly RequestDelegate _next;
        private readonly ILogger<VotingMiddleware> _logger;

        public VotingMiddleware(RequestDelegate next, ILogger<VotingMiddleware> logger, string authKey)
        {
            _logger = logger;
            _authKey = authKey;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            async Task RespondAsync(int statusCode, string responseBody)
            {
                httpContext.Response.StatusCode = statusCode;
                httpContext.Response.ContentType = "application/json";
                await httpContext.Response.WriteAsync(responseBody).ConfigureAwait(false);
                await httpContext.Response.CompleteAsync().ConfigureAwait(false);
            }

            var auth = httpContext.Request.Headers.Authorization;
            using var sr = new StreamReader(httpContext.Request.Body);
            var body = await sr.ReadToEndAsync();

            await _next(httpContext);

            if (auth[0] != _authKey)
            {
                _logger.LogError("Failure (Invalid authorization header)");
                await RespondAsync(StatusCodes.Status400BadRequest, "Invalid authorization header!");
                return;
            }

            var result = JsonConvert.DeserializeObject<Vote>(body);

            if (result is null)
            {
                _logger.LogError("Failure (Unable to deserialize vote)");
                await RespondAsync(StatusCodes.Status400BadRequest, "Invalid request body!");
                return;
            }

            await RespondAsync(StatusCodes.Status200OK, "Incoming body received succesfully.");
            _logger.LogInformation("Success (Receive vote for {}", result.UserId);

            var user = await UserEntity.GetAsync(result.UserId);

            // assign constant variables for these non-constant fields
            var time = DateTime.UtcNow;
            var oldTime = user.LastVotedAt;

            // reset the monthly votes.
            if (oldTime.AddMonths(1).Month == time.Month)
            {
                // if there are more than 1 votes last month.
                if (user.MonthlyVotes is not 0)
                {
                    var flags = user.Flags;
                    // if the user didnt vote the full month last month.
                    if (DateTime.DaysInMonth(user.LastVotedAt.Year, user.LastVotedAt.Month) != user.MonthlyVotes)
                    {
                        // remove all the old flags, as the user did not vote the whole month and cannot be reassigned as top voter.
                        flags.RemoveAll(x => x.Type is FlagType.TopVoter);
                        user.Flags = flags;
                    }
                    else // if they did
                    {
                        // dont add if the user already has the flag.
                        if (!user.Flags.Contains(UserFlag.TopVoter))
                        {
                            flags.Add(UserFlag.TopVoter);
                            user.Flags = flags;
                        }
                    }
                }

                user.MonthlyVotes = 1; // set the votes back to 1;
            }
            else
                user.MonthlyVotes++;

            user.LastVotedAt = DateTime.UtcNow;
            user.Votes++;

            var bumps = await BumpsEntity.GetAsync(user.UserId);

            bumps.BumpsToGive++;
        }
    }

    public class Vote
    {
        [JsonProperty("bot")]
        public ulong BotId { get; set; }

        [JsonProperty("user")]
        public ulong UserId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; } = "test";

        [JsonProperty("isWeekend")]
        public bool IsWeekend { get; set; }

        [JsonProperty("query")]
        public string? Query { get; set; }
    }
}
