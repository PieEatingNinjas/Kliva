﻿using Kliva.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kliva.Services.Interfaces
{
    public interface IStravaService
    {
        IStravaActivityService StravaActivityService { get; }

        event EventHandler<StravaServiceEventArgs> StatusEvent;

        Task GetAuthorizationCode();

        /// <summary>
        /// Get authenticated athlete
        /// </summary>
        /// <returns></returns>
        Task<Athlete> GetAthleteAsync();

        /// <summary>
        /// Get non-authenticated athlete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<AthleteSummary> GetAthleteAsync(string id);

        Task<IEnumerable<AthleteSummary>> GetFollowersAsync(string athleteId, bool authenticatedUser = true);
        Task<IEnumerable<AthleteSummary>> GetFriendsAsync(string athleteId, bool authenticatedUser = true);
        Task<IEnumerable<AthleteSummary>> GetMutualFriendsAsync(string athleteId);
        Task<IEnumerable<SegmentEffort>> GetKomsAsync(string athleteId);
        Task<Stats> GetStatsAsync(string athleteId);

        Task<Activity> GetActivityAsync(string id, bool includeEfforts);
        //Task<IEnumerable<ActivitySummary>> GetActivitiesWithAthletesAsync(int page, int perPage, ActivityFeedFilter filter);

        Task<string> GetFriendActivityDataAsync(int page, int pageSize);
        Task<string> GetMyActivityDataAsync(int page, int pageSize);

        Task UploadActivityAsync(string gpxFilePath, ActivityType activityType, string name, bool commute, bool isPrivate);


        Task<List<ActivitySummary>> HydrateActivityData(string data);

        Task GiveKudosAsync(string activityId);

        Task PostComment(string activityId, string text);

        Task PutUpdate(string activityId, string name, bool commute, bool isPrivate, string gearID);

        Task<List<ClubSummary>> GetClubsAsync();
        Task<Club> GetClubAsync(string id);

        Task<Segment> GetSegmentAsync(string segmentId);
        Task<SegmentEffort> GetSegmentEffortAsync(string segmentEffortId);
        Task<List<SegmentSummary>> GetStarredSegmentsAsync();
        Task<List<SegmentSummary>> GetStarredSegmentsAsync(string athleteId);

        AthleteSummary ConsolidateWithCache(AthleteMeta athlete);
    }
}