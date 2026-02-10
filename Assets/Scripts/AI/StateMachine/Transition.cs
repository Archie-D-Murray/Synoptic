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

    public class LambdaPredicate : IPredicate {

        private readonly Func<bool> _lambda;

        public LambdaPredicate(Func<bool> lambda) {
            _lambda = lambda;
        }

        public bool Evaluate() {
            return _lambda?.Invoke() ?? false;
        }
    }
}