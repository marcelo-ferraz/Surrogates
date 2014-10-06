using Surrogates.Expressions.Classes;
using Surrogates.Mappers.Entities;

namespace Surrogates.Expressions.Methods
{
    public class EndInterferenceExpression<TBase>
        : MethodInterferenceExpression<TBase>
    {
        internal EndInterferenceExpression(
            IMappingExpression<TBase> mapper, MappingState state, InterferenceKind kind)
            : base(mapper, state, kind) { }
        
        /// <summary>
        /// Exposes the interceptor
        /// </summary>
        /// <typeparam name="TInterceptor"></typeparam>
        /// <returns></returns>
        public virtual EndExpression<TBase, TInterceptor> Using<TInterceptor>()
        {
            return 
                this.Kind == InterferenceKind.Substitution ?
                (EndExpression<TBase, TInterceptor>)new MethodReplaceExpression<TBase, TInterceptor>(Mapper, State) :
                new MethodVisitationExpression<TBase, TInterceptor>(Mapper, State);
        }
    }
}
