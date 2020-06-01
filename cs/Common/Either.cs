using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public struct Either<TLeft, TRight>
    {
        public static Either<TLeft, TRight> Left(TLeft left) => new Either<TLeft, TRight>(left);
        public static Either<TLeft, TRight> Right(TRight right) => new Either<TLeft, TRight>(right);
        public bool IsLeft { get; }
        public bool IsRight => !IsLeft;
        private readonly TLeft _leftValue;
        public TLeft LeftValue
        {
            get
            {
                if (IsLeft) return _leftValue;
                throw new InvalidOperationException($"Cannot access left when it only has right, LeftType:{typeof(TLeft).FullName}, RightType:{typeof(TRight).FullName}");
            }
        }
        private readonly TRight _rightValue;
        public TRight RightValue
        {
            get
            {
                if (IsRight) return _rightValue;
                throw new InvalidOperationException($"Cannot access right when it only has left, LeftType:{typeof(TLeft).FullName}, RightType:{typeof(TRight).FullName}");
            }
        }
        private Either(TLeft left)
        {
            _leftValue = left;
            IsLeft = true;
            _rightValue = default;
        }
        private Either(TRight right)
        {
            _rightValue = right;
            IsLeft = false;
            _leftValue = default(TLeft);
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (GetType() != obj.GetType()) return false;
            var other = (Either<TLeft, TRight>)obj;
            if (IsLeft != other.IsLeft) return false;
            return IsLeft ? Equals(LeftValue, other.LeftValue) : Equals(RightValue, other.RightValue);
        }
        public static implicit operator Either<TLeft, TRight>(TLeft left)
        {
            return Left(left);
        }
        public static implicit operator Either<TLeft, TRight>(TRight right)
        {
            return Right(right);
        }

        public static bool operator ==(Either<TLeft, TRight> a, Either<TLeft, TRight> b)
        {
            return Equals(a, b);
        }
        public static bool operator !=(Either<TLeft, TRight> a, Either<TLeft, TRight> b)
        {
            return !(a == b);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                if (IsLeft) return IsLeft.GetHashCode() * 397 + (LeftValue?.GetHashCode() ?? 0) * 397;
                else return IsRight.GetHashCode() * 397 + (RightValue?.GetHashCode() ?? 0) * 397;
            }
        }
    }
}
