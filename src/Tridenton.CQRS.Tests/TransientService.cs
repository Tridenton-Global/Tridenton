using System;
namespace Tridenton.CQRS.Tests;

public interface ITransientService
{
	int GetValue();
}

public class TransientService : ITransientService
{
	public int GetValue() => Random.Shared.Next();
}