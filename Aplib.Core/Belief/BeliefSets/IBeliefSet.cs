﻿namespace Aplib.Core.Belief.BeliefSets
{
    /// <summary>
    /// A belief set defines beliefs for an agent.
    /// </summary>
    public interface IBeliefSet : IDocumented
    {
        /// <summary>
        /// Updates all beliefs in the belief set.
        /// </summary>
        void UpdateBeliefs();
    }
}
