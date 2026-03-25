using System;

namespace AI.HSM {
    public class TransitionCondition {
        public readonly State To;
        public readonly IPredicate Predicate;

        public TransitionCondition(State to, IPredicate predicate) {
            To = to;
            Predicate = predicate;
        }

        public virtual bool Evaluate() { return Predicate.Evaluate(); }
    }

    public interface IPredicate {
        bool Evaluate();
    }

    public class AndPredicate : IPredicate {
        private readonly IPredicate _left;
        private readonly IPredicate _right;

        public AndPredicate(IPredicate left, IPredicate right) {
            _left = left;
            _right = right;
        }

        public bool Evaluate() {
            return _left.Evaluate() && _right.Evaluate();
        }
    }

    public class OrPredicate : IPredicate {
        private readonly IPredicate _left;
        private readonly IPredicate _right;

        public OrPredicate(IPredicate left, IPredicate right) {
            _left = left;
            _right = right;
        }

        public bool Evaluate() {
            return _left.Evaluate() || _right.Evaluate();
        }
    }

    public class NotPredicate : IPredicate {
        private readonly IPredicate _predicate;

        public NotPredicate(IPredicate predicate) {
            _predicate = predicate;
        }

        public bool Evaluate() {
            return !_predicate.Evaluate();
        }
    }

    public class LambdaPredicate : IPredicate {

        private readonly Func<bool> _lambda;

        public LambdaPredicate(Func<bool> lambda) {
            _lambda = lambda;
        }

        public bool Evaluate() {
            return _lambda?.Invoke() ?? false;
        }
    }

    public class RandomChancePredicate : IPredicate {
        private readonly float _chance;

        public RandomChancePredicate(float chance) {
            _chance = chance;
        }

        public bool Evaluate() {
            return UnityEngine.Random.value <= chance;
        }
    }

    public class CascadePredicate : IPredicate {
        private IPredicate[] _predicates;

        public CascadePredicate(ICollection<IPredicate> predicates) {
            _predicates = predicates.ToArray();
        }

        public bool Evaluate() {
            foreach (IPredicate predicate in _predicates) {
                if (!predicate.Evaluate()) {
                    return false;
                }
            }

            return true;
        }
    }
}