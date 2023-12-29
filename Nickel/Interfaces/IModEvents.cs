using System;

namespace Nickel;

public interface IModEvents
{
	event EventHandler<ModLoadPhase> OnModLoadPhaseFinished;
	event EventHandler<LoadStringsForLocaleEventArgs> OnLoadStringsForLocale;
}
