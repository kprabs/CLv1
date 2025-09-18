namespace CoreLib.Application.Common.DBQueries
{
    public partial class SqlQueries
    {
        public const string FeaturesConfigurationQuery = @"
            SELECT f.FeatureID, f.FeatureNKey, f.ConfigurationText
            FROM main.Feature f";
    }
}
