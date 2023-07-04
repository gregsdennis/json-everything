[![Build & Test](https://github.com/gregsdennis/json-everything/actions/workflows/dotnet-core.yml/badge.svg?branch=master&event=push)](https://github.com/gregsdennis/json-everything/actions/workflows/dotnet-core.yml)
[![Test results](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/gregsdennis/28607f2d276032f4d9a7f2c807e44df7/raw/test-results-badge.json)](https://github.com/gregsdennis/json-everything/actions?query=workflow%3A%22Build+%26+Test%22)
[![Percentage of issues still open](http://isitmaintained.com/badge/open/gregsdennis/json-everything.svg)](https://github.com/gregsdennis/json-everything/issues "Percentage of issues still open")
[![Average time to resolve an issue](http://isitmaintained.com/badge/resolution/gregsdennis/json-everything.svg)](https://github.com/gregsdennis/json-everything/issues "Average time to resolve an issue")
[![License](https://img.shields.io/github/license/gregsdennis/json-everything)](https://github.com/gregsdennis/json-everything/blob/master/LICENSE)
[![All Contributors](https://img.shields.io/github/all-contributors/gregsdennis/json-everything?color=ee8449)](#contributors)

## What is `json-everything`?

The primary philosophy behind `json-everything` is to ensure that common JSON functionality has good support in the System.Text.Json space.

The first step to that is checking whether there exist other projects that support a given function.

If so, that effort likely won't be duplicated here. A bit farther down you'll find a list of other projects that are doing some cool things with JSON.

Otherwise, it's open to consideration for this project.

<table>
<thead>
<tr>
<th width="276">Playground</th>
<th width="276">Documentation</th>
<th width="276">Blog</th>
</tr>
</thead>
<tbody>
<tr>
<td align="center"><a href="https://json-everything.net"><img src="Resources/json-logo-256.png" alt="Try it online" title="Try it online" height="50"><br>json-everything.net</a></td>
<td align="center"><a href="https://docs.json-everything.net"><img src="Resources/docs-icon.png" alt="Try it online" title="Try it online" height="50"><br>docs.json-everything.net</a></td>
<td align="center"><a href="https://blog.json-everything.net"><img src="Resources/blog-icon.png" alt="Try it online" title="Try it online" height="50"><br>blog.json-everything.net</a></td>
</tr>
</tbody>
</table>

<table>
<thead>
<tr>
<th width="276">Discuss</th>
<th width="276">Ask</th>
<th width="276">Built with</th>
</tr>
</thead>
<tbody>
<tr>
<td align="center"><a href="https://join.slack.com/t/manateeopensource/shared_invite/enQtMzU4MjgzMjgyNzU3LWZjYzAzYzY3NjY1MjY3ODI0ZGJiZjc3Nzk1MDM5NTNlMjMyOTE0MzMxYWVjMjdiOGU1NDY5OGVhMGQ5YzY4Zjg"><img src="Resources/Slack.png" alt="Discuss on Slack" title="Discuss on Slack" height="50"></a></td>
<td align="center"><a href="https://stackoverflow.com/questions/tagged/json-everything"><img src="Resources/stackoverflow.png" alt="Discuss on Slack" title="Discuss on Slack" height="50"></a></td>
<td align="center"><a href="http://www.jetbrains.com/resharper"><img src="Resources/Resharper.svg" alt="Made with Jetbrains Resharper" title="Made with Jetbrains Resharper" height="50"></a></td>
</tr>
</tbody>
</table>

## What's in the box?

There are actually multiple boxes.  Each piece of functionality has been broken out into its own library, so you can pick an choose the one(s) that you need.

<table>
<tbody>
<tr>
<td width="415">JsonSchema.Net</td>
<td width="207.5"><a href="https://www.nuget.org/packages/JsonSchema.Net/"><img alt="NuGet version" src="https://img.shields.io/nuget/vpre/JsonSchema.Net.svg?svg=true"></img></td>
<td width="207.5"><img alt="NuGet version" src="https://img.shields.io/nuget/dt/JsonSchema.Net.svg?svg=true"></img></a></td>
</tr>
<tr>
<td>JsonSchema.Net.Generation</td>
<td><a href="https://www.nuget.org/packages/JsonSchema.Net.Generation/"><img alt="NuGet version" src="https://img.shields.io/nuget/vpre/JsonSchema.Net.Generation.svg?svg=true"></img></td>
<td><img alt="NuGet version" src="https://img.shields.io/nuget/dt/JsonSchema.Net.Generation.svg?svg=true"></img></a></td>
</tr>
<tr>
<td>JsonSchema.Net.DataGeneration</td>
<td><a href="https://www.nuget.org/packages/JsonSchema.Net.DataGeneration/"><img alt="NuGet version" src="https://img.shields.io/nuget/vpre/JsonSchema.Net.DataGeneration.svg?svg=true"></img></td>
<td><img alt="NuGet version" src="https://img.shields.io/nuget/dt/JsonSchema.Net.DataGeneration.svg?svg=true"></img></a></td>
</tr>
<tr>
<td>JsonSchema.Net.Data</td>
<td><a href="https://www.nuget.org/packages/JsonSchema.Net.Data/"><img alt="NuGet version" src="https://img.shields.io/nuget/vpre/JsonSchema.Net.Data.svg?svg=true"></img></td>
<td><img alt="NuGet version" src="https://img.shields.io/nuget/dt/JsonSchema.Net.Data.svg?svg=true"></img></a></td>
</tr>
<tr>
<td>JsonSchema.Net.OpenApi</td>
<td><a href="https://www.nuget.org/packages/JsonSchema.Net.OpenApi/"><img alt="NuGet version" src="https://img.shields.io/nuget/vpre/JsonSchema.Net.OpenApi.svg?svg=true"></img></td>
<td><img alt="NuGet version" src="https://img.shields.io/nuget/dt/JsonSchema.Net.OpenApi.svg?svg=true"></img></a></td>
</tr>
<tr>
<td>JsonSchema.Net.UniqueKeys</td>
<td><a href="https://www.nuget.org/packages/JsonSchema.Net.UniqueKeys/"><img alt="NuGet version" src="https://img.shields.io/nuget/vpre/JsonSchema.Net.UniqueKeys.svg?svg=true"></img></td>
<td><img alt="NuGet version" src="https://img.shields.io/nuget/dt/JsonSchema.Net.UniqueKeys.svg?svg=true"></img></a></td>
</tr>
<tr>
<td>JsonPointer.Net</td>
<td><a href="https://www.nuget.org/packages/JsonPointer.Net/"><img alt="NuGet version" src="https://img.shields.io/nuget/vpre/JsonPointer.Net.svg?svg=true"></img></td>
<td><img alt="NuGet version" src="https://img.shields.io/nuget/dt/JsonPointer.Net.svg?svg=true"></img></a></td>
</tr>
<tr>
<td>JsonPath.Net</td>
<td><a href="https://www.nuget.org/packages/JsonPath.Net/"><img alt="NuGet version" src="https://img.shields.io/nuget/vpre/JsonPath.Net.svg?svg=true"></img></td>
<td><img alt="NuGet version" src="https://img.shields.io/nuget/dt/JsonPath.Net.svg?svg=true"></img></a></td>
</tr>
<tr>
<td>JsonPatch.Net</td>
<td><a href="https://www.nuget.org/packages/JsonPatch.Net/"><img alt="NuGet version" src="https://img.shields.io/nuget/vpre/JsonPatch.Net.svg?svg=true"></img></td>
<td><img alt="NuGet version" src="https://img.shields.io/nuget/dt/JsonPatch.Net.svg?svg=true"></img></a></td>
</tr>
<tr>
<td>JsonLogic</td>
<td><a href="https://www.nuget.org/packages/JsonLogic/"><img alt="NuGet version" src="https://img.shields.io/nuget/vpre/JsonLogic.svg?svg=true"></img></td>
<td><img alt="NuGet version" src="https://img.shields.io/nuget/dt/JsonLogic.svg?svg=true"></img></a></td>
</tr>
<tr>
<td>Json.More.Net</td>
<td><a href="https://www.nuget.org/packages/Json.More.Net/"><img alt="NuGet version" src="https://img.shields.io/nuget/vpre/Json.More.Net.svg?svg=true"></img></td>
<td><img alt="NuGet version" src="https://img.shields.io/nuget/dt/Json.More.Net.svg?svg=true"></img></a></td>
</tr>
<tr>
<td>Yaml2JsonNode</td>
<td><a href="https://www.nuget.org/packages/Yaml2JsonNode/"><img alt="NuGet version" src="https://img.shields.io/nuget/vpre/Yaml2JsonNode.svg?svg=true"></img></td>
<td><img alt="NuGet version" src="https://img.shields.io/nuget/dt/Yaml2JsonNode.svg?svg=true"></img></a></td>
</tr>
</tbody>
</table>

- [JSON Schema (drafts 6 and higher)](https://json-schema.org)
  - Generation of schemas from .Net types supported in an additional library
  - Random instance data generation (powered by [Bogus](https://github.com/bchavez/Bogus))
  - [OpenApi 3.1](https://www.openapis.org/) vocabulary extension
  - A vocabulary for accessing instance and external data
  - A vocabulary for validating item uniqueness based on specific item values
- [JSON Path (RFC in progress)](https://github.com/jsonpath-standard/internet-draft) (.Net Standard 2.1) (please see issue #309 regarding future support)
- [JSON Patch (RFC 6902)](https://tools.ietf.org/html/rfc6902)
- [JsonLogic](https://jsonlogic.com) (.Net Standard 2.1)
- [JSON Pointer (RFC 6901)](https://tools.ietf.org/html/rfc6901)
- [Relative JSON Pointer](https://tools.ietf.org/id/draft-handrews-relative-json-pointer-00.html)
- Json.More.Net
  - Just some general-use extensions that probably should be included in System.Text.Json(.Nodes) but aren't
- Yaml2JsonNode
  - When you use YamlDotNet to parse YAML text, this library will convert it to `JsonNode` for you!  Then you can use that data wherever you already have support for `JsonNode` (like all of those ☝️).

Error message translations for JsonSchema.Net available in:

- Spanish `es-es`
- Norwegian `nb-no`
- Swedish `sv-se`

***NOTE** Each language pack is provided by its own Nuget package.*

You can also view JSON Schema Test Suite results on [Bowtie](https://bowtie-json-schema.github.io/bowtie), which runs the test suite against multiple implementations across different platforms.

***DISCLAIMER** My library, `JsonSchema.Net` is not related to or associated with the website https://jsonschema.net, except that they are both excellent JSON Schema tools.*

## System.Text.Json support by other projects

If you don't find what you're looking for here, please try one of these excellent projects:

- [JsonCons.Net](https://github.com/danielaparker/JsonCons.Net) by [@danielaparker](https://github.com/danielaparker)
  - JSON Pointer
  - JSON Patch
  - JSON Merge Patch
  - JSON Path
  - JMES Path
- (more to come)

If you use JSON to do something that is not covered by a library in this suite or one of the above projects, feel free to [create a feature issue](https://github.com/gregsdennis/json-everything/issues/new?assignees=&labels=feature&template=Feature_request.md).

If you have or know of another project that extends System.Text.Json to do cool things, I'd like to list it here, so please [create a general issue](https://github.com/gregsdennis/json-everything/issues/new?assignees=&labels=question&template=Question.md) to let me know about it.

## Contributors

This repository follows the [All Contributors specification](https://allcontributors.org/).

Please see the [CONTRIBUTING](./CONTRIBUTING.md) file for more information.

