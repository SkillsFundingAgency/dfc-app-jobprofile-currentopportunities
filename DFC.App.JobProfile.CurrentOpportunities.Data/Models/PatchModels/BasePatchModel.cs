﻿using DFC.App.JobProfile.CurrentOpportunities.Data.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.Models.PatchModels
{
    public abstract class BasePatchModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid JobProfileId { get; set; }

        [Required]
        public MessageAction ActionType { get; set; }

        [Required]
        public long SequenceNumber { get; set; }
    }
}