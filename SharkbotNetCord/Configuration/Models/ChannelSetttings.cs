﻿namespace SharkbotNetCord.Configuration.Models
{
    [Serializable]
    public class ChannelSettings
    {
        public double TargetedResponseConfidenceThreshold { get; set; }
        public double ReactionConfidenceThreshold { get; set; }
    }
}
