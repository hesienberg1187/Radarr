﻿// ReSharper disable RedundantUsingDirective
// ReSharper disable RedundantUsingDirective
using System;
using FizzWare.NBuilder;
using NUnit.Framework;
using NzbDrone.Core.Model;
using NzbDrone.Core.Repository;
using NzbDrone.Core.Test.Framework;

namespace NzbDrone.Core.Test
{
    [TestFixture]
    // ReSharper disable InconsistentNaming
    public class EpisodeStatusTest : TestBase
    {
        [Test]
        [TestCase(1, false, false, EpisodeStatusType.NotAired)]
        [TestCase(-2, false, false, EpisodeStatusType.Missing)]
        [TestCase(1, true, false, EpisodeStatusType.Ready)]
        [TestCase(1, false, true, EpisodeStatusType.Ignored)]
        public void no_grab_date(int offsetDays, bool hasEpisodes, bool ignored, EpisodeStatusType status)
        {
            Episode episode = Builder<Episode>.CreateNew()
                .With(e => e.Season = Builder<Season>.CreateNew()
                                          .With(s => s.Monitored = true).Build())
                .With(e => e.AirDate = DateTime.Now.AddDays(offsetDays))
                .With(e => e.Ignored = ignored)
                .With(e => e.EpisodeFileId = 0)
                .With(e => e.GrabDate = null)
                .Build();

            if (hasEpisodes)
            {
                episode.EpisodeFileId = 12;
            }

            Assert.AreEqual(status, episode.Status);
        }


        [Test]
        [TestCase(1, false, false, EpisodeStatusType.NotAired)]
        [TestCase(-2, false, false, EpisodeStatusType.Missing)]
        [TestCase(1, true, false, EpisodeStatusType.Ready)]
        [TestCase(1, false, true, EpisodeStatusType.Ignored)]
        public void old_grab_date(int offsetDays, bool hasEpisodes, bool ignored,
                                                 EpisodeStatusType status)
        {
            Episode episode = Builder<Episode>.CreateNew()
                .With(e => e.Season = Builder<Season>.CreateNew()
                                          .With(s => s.Monitored = true).Build()).With(
                                              e => e.AirDate = DateTime.Now.AddDays(offsetDays))
                .With(e => e.Ignored = ignored)
                .With(e => e.EpisodeFileId = 0)
                .With(e => e.GrabDate = DateTime.Now.AddDays(-1).AddHours(-1))
                .Build();

            if (hasEpisodes)
            {
                episode.EpisodeFileId = 12;
            }

            Assert.AreEqual(status, episode.Status);
        }


        [Test]
        [TestCase(1, false, false, EpisodeStatusType.Downloading)]
        [TestCase(-2, false, false, EpisodeStatusType.Downloading)]
        [TestCase(1, true, false, EpisodeStatusType.Downloading)]
        [TestCase(1, true, true, EpisodeStatusType.Downloading)]
        [TestCase(1, false, true, EpisodeStatusType.Downloading)]
        public void recent_grab_date(int offsetDays, bool hasEpisodes, bool ignored,
                                                    EpisodeStatusType status)
        {
            Episode episode = Builder<Episode>.CreateNew()
                .With(e => e.Season = Builder<Season>.CreateNew()
                                          .With(s => s.Monitored = true).Build())
                .With(e => e.AirDate = DateTime.Now.AddDays(offsetDays))
                .With(e => e.Ignored = ignored)
                .With(e => e.EpisodeFileId = 0)
                .With(e => e.GrabDate = DateTime.Now.AddDays(-1))
                .Build();

            if (hasEpisodes)
            {
                episode.EpisodeFileId = 12;
            }

            Assert.AreEqual(status, episode.Status);
        }

        [Test]
        [TestCase(1, false, false, EpisodeStatusType.Ignored)]
        [TestCase(-2, false, false, EpisodeStatusType.Ignored)]
        [TestCase(1, false, true, EpisodeStatusType.Ignored)]
        public void skipped_season(int offsetDays, bool hasEpisodes, bool ignored, EpisodeStatusType status)
        {
            Episode episode = Builder<Episode>.CreateNew()
                .With(e => e.AirDate = DateTime.Now.AddDays(offsetDays))
                .With(e => e.Ignored = ignored)
                .With(e => e.EpisodeFileId = 0)
                .With(e => e.GrabDate = null)
                .With(e => e.Season = Builder<Season>.CreateNew()
                                          .With(s => s.Monitored == false).Build())

                .Build();

            if (hasEpisodes)
            {
                episode.EpisodeFileId = 12;
            }

            Assert.AreEqual(status, episode.Status);
        }


        [Test]
        public void low_air_date()
        {
            Episode episode = Builder<Episode>.CreateNew()
                .With(e => e.AirDate = DateTime.Now.AddYears(-200))
                .With(e => e.Ignored = false)
                .With(e => e.EpisodeFileId = 0)
                .With(e => e.GrabDate = null)
                .With(e => e.Season = Builder<Season>.CreateNew()
                                          .With(s => s.Monitored = true).Build())
                .Build();


            Assert.AreEqual(EpisodeStatusType.NotAired, episode.Status);
        }
    }
}