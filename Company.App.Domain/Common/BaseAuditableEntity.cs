namespace Company.App.Domain.Common
{
    public abstract class BaseAuditableEntity : BaseEntity
    {
        public DateTime DateCreation { get; set; }
        public DateTime? DateModification { get; set; }
        public bool RowStatus = true;
    }
}
