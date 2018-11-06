namespace Stripe
{
    /// <summary>
    /// Resources that implement this interface can be used as external accounts, i.e. they can
    /// be attached to a Stripe account to receive payouts.
    /// </summary>
    public interface IExternalAccount : IStripeEntity, IHasId, IHasObject
    {
        Account Account { get; }

        string AccountId { get; }
    }
}
