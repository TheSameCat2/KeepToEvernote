using KeepToEvernote;
using System.Reflection.Metadata;
using System.Security;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;

// The MIT License (MIT)
// Copyright © 2022 TheSameCat

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.


Console.WriteLine("KeepToEvernote - Convert Your Google(R) Keep notes to Evernote Notes");
Console.WriteLine("Copyright 2022 by TheSameCat");
Console.WriteLine("Released under the MIT License.");

if (args.Length != 1)
{
    PrintUsage();
    return;
}

var files = from file in 
                Directory.EnumerateFiles(Directory.GetCurrentDirectory(), args[0], 
                SearchOption.TopDirectoryOnly) select file;

foreach (var file in files)
{
    var jsonContents = File.ReadAllText(file);
    KeepNote? keepNote = JsonSerializer.Deserialize<KeepNote>(jsonContents);

    if (keepNote != null)
    {
        string newFileName = Path.ChangeExtension(file, ".enex");

        string? newFileContents = ConvertJsonToEvernoteXml(keepNote);

        File.WriteAllText(newFileName, newFileContents);
    }
}

Console.WriteLine("Done! {0} files written!", files.Count());

void PrintUsage()
{
    Console.WriteLine("Usage: KeepToEvernote [input pattern].json");
}

string? ConvertJsonToEvernoteXml(KeepNote keepNote)
{
    XmlDocument document = new XmlDocument();
    var docType = document.CreateDocumentType("en-export", null, "http://xml.evernote.com/pub/evernote-export4.dtd", null);
    document.AppendChild(docType);

    var en_export = document.CreateElement("en-export");
    en_export.SetAttribute("export-date", DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ"));
    en_export.SetAttribute("application", "Evernote");
    en_export.SetAttribute("version", "10.49.4"); // Current as of writing
    document.AppendChild(en_export);

    var note = document.CreateElement("note");
    en_export.AppendChild(note);

    var title = document.CreateElement("title");
    title.InnerText = keepNote.title;
    note.AppendChild(title);

    var created = document.CreateElement("created");
    created.InnerText = KeepNote.FromUsecSinceUnixEpoch(keepNote.createdTimestampUsec).ToString("yyyyMMddTHHmmssZ");
    note.AppendChild(created);

    var updated = document.CreateElement("updated");
    updated.InnerText = KeepNote.FromUsecSinceUnixEpoch(keepNote.userEditedTimestampUsec).ToString("yyyyMMddTHHmmssZ");
    note.AppendChild(updated);

    if (keepNote.labels != null)
    {
        foreach (var label in keepNote.labels)
        {
            if (label != null)
            {
                var tag = document.CreateElement("tag");
                tag.InnerText = label["name"];
                note.AppendChild(tag);
            }
        }
    }

    var note_attributes = document.CreateElement("note-attributes");
    var author = document.CreateElement("author");
    author.InnerText = ""; // Possibly accept command line arg for this later?
    note_attributes.AppendChild(author);
    note.AppendChild(note_attributes);

    var content = document.CreateElement("content");
    XmlDocument contentData = new();
    { 
        var contentDocType = contentData.CreateDocumentType("en-note", null, "http://xml.evernote.com/pub/enml2.dtd", null);
        contentData.AppendChild(contentDocType);

        var en_note = contentData.CreateElement("en-note");

        if (keepNote.textContent != null)
        {
            string? contentText = keepNote.textContent;
            contentText = contentText.Replace("\n", @"<br />");
            en_note.InnerXml = contentText;
            
        }
        else if (keepNote.listContent!= null)
        {
            string listText = @"<ul style=""--en-todo:true;"">";
            foreach (var item in keepNote.listContent)
            {
                listText += String.Format("<li style=\"--en-checked:{0};\">", ((JsonElement)item["isChecked"]).GetBoolean());
                listText += ((JsonElement)item["text"]).GetString();
                listText += @"</li>";

            }
            listText += @"</ul>";
            en_note.InnerXml = listText;
        }
        contentData.AppendChild(en_note);


        StringWriter noteStringWriter = new StringWriter();
        XmlTextWriter noteXmlWriter = new XmlTextWriter(noteStringWriter);
        contentData.WriteTo(noteXmlWriter);
        string en_note_contents = noteStringWriter.ToString();
        var contentCData = document.CreateCDataSection(en_note_contents);
        content.AppendChild(contentCData);
    }

    note.AppendChild(content);

    StringWriter documentStringWriter = new StringWriter();
    XmlTextWriter documentXmlWriter = new XmlTextWriter(documentStringWriter);
    document.WriteTo(documentXmlWriter);
    return documentStringWriter.ToString();
}
