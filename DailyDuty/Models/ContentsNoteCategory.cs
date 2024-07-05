// ReSharper disable All

using Lumina.Data;
using Lumina.Text;

namespace Lumina.Excel.GeneratedSheets;

[Sheet( "ContentsNoteCategory" )]
public partial class ContentsNoteCategory : ExcelRow {
	public SeString CategoryName { get; set; }
        
	public override void PopulateData( RowParser parser, GameData gameData, Language language ) {
		base.PopulateData( parser, gameData, language );

		CategoryName = parser.ReadColumn<SeString>( 1 )!;
	}
}