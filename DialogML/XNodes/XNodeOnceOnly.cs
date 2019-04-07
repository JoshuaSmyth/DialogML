using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogML.XNodes
{
    public enum OnceOnlyConfig : byte
    {
        NoChildren = 0,
        SingleChildTrue = 1,
        SingleChildFalse = 2,
        TwinChildrenTrueFalse = 3,
        TwinChildrenFalseTrue = 4
    }

    public class XNodeOnceOnly : XmlNode
    {
        public override void OnProcessElement(ScriptIds ids, string name, string value)
        {
            // Should this id stuff be handled already?
            var loweredName = name.ToLower();
            if(loweredName == "id")
            {
                Id = ids.GetGuidByIndex(value);
            }
        }

        public override void WriteBytes(CompileContext ctx)
        //public override void WriteBytes(BinaryWriter bw, string filename, ref StringTable stringTable, ref ReferencesTable referencesTable) // TODO Pass string table
        {
            this.WriteHeader(ctx.bw, XNodeType.OnceOnly);


            OnceOnlyConfig config = OnceOnlyConfig.NoChildren;
            var childrenType = new char[2];

            if (Children.Count() > 2)
            {
                throw new Exception("Only Only Node can have max 2 children");
            }
            for(int i=0; i< childrenType.Count(); i++)
            {
                if (Children[i] is XNodeCaseFalse)
                {
                    childrenType[i] = 'f';
                }
                if (Children[i] is XNodeCaseTrue)
                {
                    childrenType[i] = 't';
                }
            }
            if (childrenType[0] == 'f' && childrenType[1] == 't')
            {
                config = OnceOnlyConfig.TwinChildrenFalseTrue;
            }
            else if (childrenType[0] == 't' && childrenType[1] == 'f')
            {
                config = OnceOnlyConfig.TwinChildrenTrueFalse;
            }
            else if (childrenType[0] == 't')
            {
                config = OnceOnlyConfig.SingleChildTrue;
            }
            else if (childrenType[0] == 'f')
            {
                config = OnceOnlyConfig.SingleChildFalse;
            }

            ctx.bw.Write((byte)config);
        }
    }
}
