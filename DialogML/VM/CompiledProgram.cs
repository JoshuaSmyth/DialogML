using DialogML;
using DialogML.RNodes;
using System;
using System.Collections.Generic;

public class CompiledProgram
{
    public StringTable stringTable = new StringTable();

    public Dictionary<Guid, CompiledScript> Scripts = new Dictionary<Guid, CompiledScript>();

    public void LoadStringTable(byte[] bytes)
    {
        stringTable.Deserialise(bytes);
    }

    public void LoadScriptFile(byte[] bytes)
    {
        var script = new CompiledScript();
        script.Deserialise(bytes);
    }
}