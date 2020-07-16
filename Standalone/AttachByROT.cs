using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Standalone
{
    class AttachByROT
    {
        [DllImport("ole32.dll")]

        static extern int CreateBindCtx(

    uint reserved,

    out IBindCtx ppbc);



        [DllImport("ole32.dll")]

        public static extern void GetRunningObjectTable(

            int reserved,

            out IRunningObjectTable prot);



        // Requires Using System.Runtime.InteropServices.ComTypes

        // Get all running instance by querying ROT

        private List<object> GetRunningInstances(string[] progIds)

        {

            List<string> clsIds = new List<string>();



            // get the app clsid

            foreach (string progId in progIds)

            {

                Type type = Type.GetTypeFromProgID(progId);



                if (type != null)

                    clsIds.Add(type.GUID.ToString().ToUpper());

            }



            // get Running Object Table ...

            IRunningObjectTable Rot = null;

            GetRunningObjectTable(0, out Rot);

            if (Rot == null)

                return null;



            // get enumerator for ROT entries

            IEnumMoniker monikerEnumerator = null;

            Rot.EnumRunning(out monikerEnumerator);



            if (monikerEnumerator == null)

                return null;



            monikerEnumerator.Reset();



            List<object> instances = new List<object>();



            IntPtr pNumFetched = new IntPtr();

            IMoniker[] monikers = new IMoniker[1];



            // go through all entries and identifies app instances

            while (monikerEnumerator.Next(1, monikers, pNumFetched) == 0)

            {

                IBindCtx bindCtx;

                CreateBindCtx(0, out bindCtx);

                if (bindCtx == null)

                    continue;



                string displayName;

                monikers[0].GetDisplayName(bindCtx, null, out displayName);



                foreach (string clsId in clsIds)

                {

                    if (displayName.ToUpper().IndexOf(clsId) > 0)

                    {

                        object ComObject;

                        Rot.GetObject(monikers[0], out ComObject);



                        if (ComObject == null)

                            continue;



                        instances.Add(ComObject);

                        break;

                    }

                }

            }



            return instances;

        }



        public void TestROT()

        {

            // Look for acad 2009 & 2010 & 2014

            string[] progIds =

            {

                "CorelDRAW.Application.18"

            };



            List<object> instances = GetRunningInstances(progIds);



            foreach (object acadObj in instances)

            {

                try

                {

                    dynamic corelApp = acadObj as dynamic;
                    dynamic sr = corelApp.ActiveSelection.Shapes.All();

                    for (int i = 1; i <= sr.Count; i++)
                    {
                        sr[i].Name = "ts";
                    }

                }

                catch

                {



                }

            }

        }
    }
}
