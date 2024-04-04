using System;
using Json.Pointer;

namespace FunctionalJsonSchema;

public class RefResolutionException : Exception
{
	public Uri BaseUri { get; }
	public string? Anchor { get; }
	public bool IsDynamic { get; }
	public JsonPointer? Location { get; }

	public RefResolutionException(Uri baseUri, string? anchor = null, bool isDynamic = false)
		: base($"Could not resolve {Format(baseUri, anchor, isDynamic)}")
	{
		BaseUri = baseUri;
		Anchor = anchor;
		IsDynamic = isDynamic;
	}

	public RefResolutionException(Uri baseUri, JsonPointer location)
		: base($"Could not resolve schema '{baseUri}{location.ToString(JsonPointerStyle.UriEncoded)}'")
	{
		BaseUri = baseUri;
		Location = location;
	}

	private static string Format(Uri baseUri, string? anchor, bool isDynamic)
	{
		return anchor is null 
			? $"'{baseUri}'" :
			$"{(isDynamic ? "dynamic " : string.Empty)}anchor '{anchor}' in schema '{baseUri}'";
	}
}