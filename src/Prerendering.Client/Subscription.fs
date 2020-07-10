namespace Prerendering.Client

module Subscriptions =
    open System

    let none = { new IDisposable with member x.Dispose () = () }


namespace Bolero

module Html =
    open Microsoft.AspNetCore.Components
    open Bolero.Html
    open System
    open Prerendering.Client

    type SubscribeComponent () =
        inherit Component ()

        [<Parameter>]
        member val Subscription =
            Subscriptions.none with get, set

        [<Parameter>]
        member val ChildContent =
            Unchecked.defaultof<RenderFragment> with get, set

        override this.Render () =
            fragment this.ChildContent

        interface System.IDisposable with
            member this.Dispose() =
                printf "Dispose!"
                this.Subscription.Dispose ()


    let subscribe (subscription : IDisposable) =
        comp<SubscribeComponent> [ "Subscription" => subscription ]


namespace Elmish
module Sub =
    open System
    open System.Timers
    open Prerendering.Client


    let none : IDisposable * Cmd<'msg> =
        Subscriptions.none, Cmd.none

    let interval milliseconds msg =
        let timer = new Timer (milliseconds)
        timer :> IDisposable, Cmd.ofSub <| fun dispatch ->
            timer.add_Elapsed (fun _ e ->
                printf "Interval!"
                dispatch <| msg e.SignalTime)
            timer.Start ()