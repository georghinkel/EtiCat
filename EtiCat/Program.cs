using CommandLine;
using EtiCat.Verbs;

Environment.ExitCode = Parser.Default.ParseArguments(args,
    typeof(ApplyVerb),
    typeof(CheckVerb),
    typeof(ExplainVerb)).MapResult((VerbBase v) => v.Execute(), _ => 2);