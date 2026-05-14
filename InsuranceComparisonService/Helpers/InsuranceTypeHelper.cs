namespace InsuranceComparisonService.Helpers
{
    public static class InsuranceTypeHelper
    {
        public static string GetDisplayName(Models.InsuranceType type)
        {
            return type switch
            {
                Models.InsuranceType.Kasko    => "Каско",
                Models.InsuranceType.Health   => "Здравна",
                Models.InsuranceType.Civil    => "Гражданска отговорност",
                Models.InsuranceType.Property => "Имуществена",
                _                             => type.ToString()
            };
        }

        public static string GetBadgeClass(Models.InsuranceType type)
        {
            return type switch
            {
                Models.InsuranceType.Kasko    => "bg-primary",
                Models.InsuranceType.Health   => "bg-danger",
                Models.InsuranceType.Civil    => "bg-warning text-dark",
                Models.InsuranceType.Property => "bg-success",
                _                             => "bg-secondary"
            };
        }

        public static string GetIconClass(Models.InsuranceType type)
        {
            return type switch
            {
                Models.InsuranceType.Kasko    => "bi-car-front",
                Models.InsuranceType.Health   => "bi-heart-pulse",
                Models.InsuranceType.Civil    => "bi-file-earmark-text",
                Models.InsuranceType.Property => "bi-house",
                _                             => "bi-shield"
            };
        }
    }
}
