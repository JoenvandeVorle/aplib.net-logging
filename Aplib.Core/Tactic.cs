﻿using System;
using System.Collections.Generic;

namespace Aplib.Core
{
    /// <summary>
    /// Represents the type of a tactic.
    /// </summary>
    public enum TacticType
    {
        Primitive,
        FirstOf,
        AnyOf,
    }

    /// <summary>
    /// Represents a tactic in the Aplib.Core namespace.
    /// </summary>
    public class Tactic
    {
        /// <summary>
        /// Gets or sets the parent tactic.
        /// </summary>
        private Tactic? _parent = null;

        /// <summary>
        /// Gets or sets the type of the tactic.
        /// </summary>
        public TacticType TacticType { get; set; }

        /// <summary>
        /// Gets or sets the sub-tactics of the tactic.
        /// </summary>
        private LinkedList<Tactic> _subTactics { get; set; }

        /// <summary>
        /// Gets or sets the guard of the tactic.
        /// </summary>
        protected Func<bool> Guard { get; set; } = () => true;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tactic"/> class with the specified tactic type and sub-tactics.
        /// </summary>
        /// <param name="tacticType">The type of the tactic.</param>
        /// <param name="subTactics">The sub-tactics of the tactic.</param>
        public Tactic(TacticType tacticType, List<Tactic> subTactics)
        {
            TacticType = tacticType;
            _subTactics = new();

            foreach (Tactic tactic in subTactics)
            {
                tactic._parent = this;
                _ = _subTactics.AddLast(tactic);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tactic"/> class with the specified tactic type, sub-tactics, and guard.
        /// </summary>
        /// <param name="tacticType">The type of the tactic.</param>
        /// <param name="subTactics">The sub-tactics of the tactic.</param>
        /// <param name="guard">The guard of the tactic.</param>
        public Tactic(TacticType tacticType, List<Tactic> subTactics, Func<bool> guard)
        	: this(tacticType, subTactics)
        {
            Guard = guard;
        }

        /// <summary>
        /// Gets the next tactic in the hierarchy.
        /// </summary>
        /// <returns>The next tactic, or null if there is no next tactic.</returns>
        public Tactic? GetNextTactic()
        {
            if (_parent == null)
                return null;

            if (TacticType == TacticType.Primitive)
            {
                PrimitiveTactic tactic = (PrimitiveTactic)this;

                return tactic;
            }

            return _parent.GetNextTactic();
        }

        /// <summary>
        /// Gets the first enabled primitive actions.
        /// </summary>
        /// <returns>A list of primitive tactics that are enabled.</returns>
        public List<PrimitiveTactic> GetFirstEnabledActions()
        {
            List<PrimitiveTactic> primitiveTactics = new();

            switch (TacticType)
            {
                case TacticType.FirstOf:
                    foreach (Tactic subTactic in _subTactics)
                    {
                        primitiveTactics = subTactic.GetFirstEnabledActions();

                        if (primitiveTactics.Count > 0)
                            break;
                    }

                    break;
                case TacticType.AnyOf:
                    foreach (Tactic subTactic in _subTactics)
                    {
                        primitiveTactics.AddRange(subTactic.GetFirstEnabledActions());
                    }

                    break;
                case TacticType.Primitive:
                    PrimitiveTactic tactic = (PrimitiveTactic)this;

                    if (tactic.IsActionable())
                        primitiveTactics.Add(tactic);

                    break;
            }

            return primitiveTactics;
        }

        /// <summary>
        /// Determines whether the tactic is actionable.
        /// </summary>
        /// <returns>True if the tactic is actionable, false otherwise.</returns>
        public virtual bool IsActionable() => Guard();
    }

    /// <summary>
    /// Represents a primitive tactic in the Aplib.Core namespace.
    /// </summary>
    public class PrimitiveTactic : Tactic
    {
        /// <summary>
        /// Gets or sets the action of the primitive tactic.
        /// </summary>
        private readonly Action _action;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimitiveTactic"/> class with the specified action.
        /// </summary>
        /// <param name="action">The action of the primitive tactic.</param>
        public PrimitiveTactic(Action action) : base(TacticType.Primitive, new()) => _action = action;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimitiveTactic"/> class with the specified action.
        /// </summary>
        /// <param name="action">The action of the primitive tactic.</param>
        /// <param name="guard">The guard of the tactic.</param>
        public PrimitiveTactic(Action action, Func<bool> guard) : base(TacticType.Primitive, new(), guard) => _action = action;

        public override bool IsActionable() => Guard() && _action.IsActionable();
    }
}
