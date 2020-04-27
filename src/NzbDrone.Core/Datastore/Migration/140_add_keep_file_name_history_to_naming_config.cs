using FluentMigrator;
using NzbDrone.Core.Datastore.Migration.Framework;

namespace NzbDrone.Core.Datastore.Migration
{
    [Migration(140)]
    public class add_keep_file_name_history_to_naming_config : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Alter.Table("NamingConfig").AddColumn("KeepFileNameHistory").AsBoolean().WithDefaultValue(false);
            Update.Table("NamingConfig").Set(new { KeepFileNameHistory = false }).AllRows();
        }
    }
}