﻿using System;
using System.Collections.Generic;
using Action = Aplib.Core.Intent.Actions.Action;

namespace Aplib.Core.Intent.Tactics
{
    /// <summary>
    /// Represents a tactic that executes any of the provided sub-tactics.
    /// </summary>
    public class AnyOfTactic : Tactic
    {
        /// <summary>
        /// Gets or sets the sub-tactics of the tactic.
        /// </summary>
        protected LinkedList<Tactic> SubTactics { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnyOfTactic"/> class with the specified sub-tactics.
        /// </summary>
        /// <param name="metadata">
        /// Metadata about this tactic, used to quickly display the tactic in several contexts.
        /// </param>
        /// <param name="subTactics">The list of sub-tactics.</param>
        public AnyOfTactic(Metadata? metadata = null, params Tactic[] subTactics)
            : base(metadata)
        {
            SubTactics = new();

            foreach (Tactic tactic in subTactics)
            {
                _ = SubTactics.AddLast(tactic);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnyOfTactic"/> class with the specified sub-tactics and guard condition.
        /// </summary>
        /// <param name="guard">The guard condition.</param>
        /// <param name="metadata">
        /// Metadata about this tactic, used to quickly display the tactic in several contexts.
        /// </param>
        /// <param name="subTactics">The list of sub-tactics.</param>
        public AnyOfTactic(Func<bool> guard, Metadata? metadata = null, params Tactic[] subTactics)
            : this(metadata, subTactics) => Guard = guard;

        /// <inheritdoc/>
        public override Action? GetAction()
        {
            List<Action> actions = new();

            foreach (Tactic subTactic in SubTactics)
            {
                Action? action = subTactic.GetAction();

                if (action is not null)
                    actions.Add(action);
            }

            if (actions.Count == 0)
                return null;

            return actions[ThreadSafeRandom.Next(actions.Count)];
        }
    }
}
