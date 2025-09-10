using CommandLine;
using EtiCat.Verbs;

Environment.ExitCode = Parser.Default.ParseArguments(args,
    typeof(ApplyVerb),
    typeof(ChangeLogVerb),
    typeof(CheckVerb),
    typeof(CiVerb),
    typeof(PackVerb),
    typeof(ProvidersVerb),
    typeof(PublishVerb),
    typeof(TagVerb)).MapResult((VerbBase v) => v.Execute(), _ => 2);