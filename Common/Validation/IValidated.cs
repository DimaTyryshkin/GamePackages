namespace GamePackages.Core.Validation
{
	public interface IValidated
	{
		void Validate(ValidationContext context);
	}
}