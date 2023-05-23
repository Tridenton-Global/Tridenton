namespace Tridenton.Core.Util;

public delegate ValueTask AsyncEventHandler(object sender, EventArgs e);
public delegate ValueTask AsyncEventHandler<TEventArgs>(object sender, TEventArgs e);