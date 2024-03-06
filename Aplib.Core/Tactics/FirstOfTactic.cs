﻿using System;
using System.Collections.Generic;

namespace Aplib.Core.Tactics
{
    /// <summary>
    /// Represents a tactic that executes the first enabled action from a list of sub-tactics.
    /// </summary>
    public class FirstOfTactic : Tactic
    {
        /// <summary>
        /// Gets or sets the sub-tactics of the tactic.
        /// </summary>
        protected LinkedList<Tactic> SubTactics { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirstOfTactic"/> class with the specified sub-tactics.
        /// </summary>
        /// <param name="subTactics">The list of sub-tactics.</param>
        public FirstOfTactic(List<Tactic> subTactics)
        {
            SubTactics = new();

            foreach (Tactic tactic in subTactics)
            {
                _ = SubTactics.AddLast(tactic);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirstOfTactic"/> class with the specified sub-tactics and guard condition.
        /// </summary>
        /// <param name="subTactics">The list of sub-tactics.</param>
        /// <param name="guard">The guard condition.</param>
        public FirstOfTactic(List<Tactic> subTactics, Func<bool> guard) : this(subTactics) => Guard = guard;

        /// <inheritdoc/>
        public override List<PrimitiveTactic> GetFirstEnabledActions()
        {
            foreach (Tactic subTactic in SubTactics)
            {
                List<PrimitiveTactic> firstOfTactics = subTactic.GetFirstEnabledActions();

                if (firstOfTactics.Count > 0)
                    return firstOfTactics;
            }

            return new();
        }
    }
}