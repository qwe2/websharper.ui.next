﻿module WebSharper.UI.Next.Tests.TextBenchmark

open IntelliFactory.WebSharper
open IntelliFactory.WebSharper.Html
open IntelliFactory.WebSharper.UI.Next.Reactive
open IntelliFactory.WebSharper.UI.Next.RDom

open System

module RVa = IntelliFactory.WebSharper.UI.Next.Reactive.Var
module RVi = IntelliFactory.WebSharper.UI.Next.Reactive.View
module RD = IntelliFactory.WebSharper.UI.Next.RDom

// Adam's idea to test efficiency of RVars and RViews:
// A bunch of interdependent text fields: so, one data source, but it has to
// propagate through a lot of other components. This should ideally be pretty
// instantaneous eventually, but will need some optimisation.
[<JavaScript>]
module TextBenchmark =
    // HACK ALERT, here be dragons
    let tryParseInt s = try
                            Int32.Parse s
                        with ex -> 0

    let rec componentLoop n prevRView =
        if n >= 0 then

            let rv = FromView (RVi.Map (fun x -> x + 1) prevRView)
            let rvi = RVi.Create rv
            let control =
                RD.InputConvert (string)
                                (tryParseInt) rv // FIXME: better handle non-bijections
            control :: componentLoop (n - 1) rvi
        else []

    let main () =
        let initVar = RVa.Create 0
        let initView = RVi.Create initVar
        let initText = RD.InputConvert (string) (tryParseInt) initVar
        RunById "main" <| ConcatTree (componentLoop 5000 initView)
        Div [ ]