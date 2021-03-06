﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Kliva.Helpers;
using Kliva.Models;
using Kliva.Services.Interfaces;
using Microsoft.Practices.ServiceLocation;

namespace Kliva.Services
{
    public class StravaClubService : IStravaClubService
    {
        private readonly ISettingsService _settingsService;
        private readonly StravaWebClient _stravaWebClient;
        private StringBuilder _errorMessage = new StringBuilder();

        //TODO: Glenn - When to Invalidate cache?
        private readonly ConcurrentDictionary<string, Task<Club>> _cachedClubTasks = new ConcurrentDictionary<string, Task<Club>>();

        public StravaClubService(ISettingsService settingsService, StravaWebClient stravaWebClient)
        {
            _settingsService = settingsService;
            _stravaWebClient = stravaWebClient;
        }

        private async Task<Club> GetClubFromServiceAsync(string clubId)
        {
            try
            {
                var accessToken = await _settingsService.GetStoredStravaAccessTokenAsync();
                string getUrl = $"{Endpoints.Club}/{clubId}?access_token={accessToken}";
                string json = await _stravaWebClient.GetAsync(new Uri(getUrl));

                return Unmarshaller<Club>.Unmarshal(json);
            }
            catch (Exception ex)
            {
#if !DEBUG
                _errorMessage.Clear();
                _errorMessage.AppendLine($"StravaClubService.GetClubFromServiceAsync - clubId {clubId}");
                _errorMessage.AppendLine(ex.Message);
                ServiceLocator.Current.GetInstance<IGoogleAnalyticsService>().Tracker.SendException(_errorMessage.ToString(), false);
#endif
            }

            return null;
        }

        /// <summary>
        /// Gets a list of clubs in which the currently authenticated athlete is a member of.
        /// </summary>
        /// <returns>The list of clubs in which the currently authenticated user is a member of.</returns>
        public async Task<List<ClubSummary>> GetClubsAsync()
        {
            try
            {
                var accessToken = await _settingsService.GetStoredStravaAccessTokenAsync();
                string getUrl = $"{Endpoints.Clubs}?access_token={accessToken}";
                string json = await _stravaWebClient.GetAsync(new Uri(getUrl));

                return Unmarshaller<List<ClubSummary>>.Unmarshal(json);
            }
            catch (Exception ex)
            {
#if !DEBUG
                _errorMessage.Clear();
                _errorMessage.AppendLine($"StravaClubService.GetClubsAsync");
                _errorMessage.AppendLine(ex.Message);
                ServiceLocator.Current.GetInstance<IGoogleAnalyticsService>().Tracker.SendException(_errorMessage.ToString(), false);
#endif
            }

            return null;
        }

        /// <summary>
        /// Gets the club which the specified id.
        /// </summary>
        /// <param name="clubId">The id of the club.</param>
        /// <returns>The Club object containing detailed information about the club.</returns>
        public Task<Club> GetClubAsync(string clubId)
        {
            return _cachedClubTasks.GetOrAdd(clubId, GetClubFromServiceAsync);
        }

        /// <summary>
        /// Gets the members of the specified club.
        /// </summary>
        /// <param name="clubId">The club's id.</param>
        /// <returns>The club's members.</returns>
        public async Task<List<AthleteSummary>> GetClubMembersAsync(string clubId)
        {
            try
            {
                var accessToken = await _settingsService.GetStoredStravaAccessTokenAsync();
                string getUrl = $"{Endpoints.Club}/{clubId}/members?access_token={accessToken}";
                string json = await _stravaWebClient.GetAsync(new Uri(getUrl));

                return Unmarshaller<List<AthleteSummary>>.Unmarshal(json);
            }
            catch (Exception ex)
            {
#if !DEBUG
                _errorMessage.Clear();
                _errorMessage.AppendLine($"StravaClubService.GetClubMembersAsync - clubId {clubId}");
                _errorMessage.AppendLine(ex.Message);
                ServiceLocator.Current.GetInstance<IGoogleAnalyticsService>().Tracker.SendException(_errorMessage.ToString(), false);
#endif
            }

            return null;
        }        
    }
}
