﻿using ToxicRagers.CarmageddonReincarnation.Formats;

using Flummery.Core.ContentPipeline;
using Flummery.Core;

namespace Flummery.Plugin.CarmageddonMaxDamage.ContentPipeline
{
    public class SetupLOLExporter : ContentExporter
    {
        public override void Export(Asset asset, string path)
        {
            Model model = (asset as Model);

            if (model.SupportingDocuments.ContainsKey("Setup"))
            {
                ((Setup)model.SupportingDocuments["Setup"]).Save(path);
            }
            else
            {
                Setup setup = new Setup();

                switch (ExportSettings.GetSetting<SetupContext>("Context"))
                {
                    case SetupContext.Vehicle:
                        setup.Settings = new VehicleSetupCode();
                        break;
                }

                setup.Save(path);
            }
        }
    }
}
