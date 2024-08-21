using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Opera
{
    /// <summary>
    /// A callback with the result of the request to the GX.Games backend. Returns the
    /// data and the info about the request: whether it is successfull or not (the
    /// argument `isOk`) and the error codes.
    /// Error codes are the codes sent from the GX.Games backend. It may be empty in case
    /// the request has been failed due to an unknown reason.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <param name="data">The data received from the backend. It contains a default value for `TData`
    /// (e.g. `null` for reference types) when `isOk` is `false`.</param>
    /// <param name="isOk">An flag indicating whether the request was successfull or not.</param>
    /// <param name="errorCodes">A list with the errors sent from GX.Games backend.</param>
    public delegate void GxGamesApiCallback<TData>(TData data, bool isOk, string[] errorCodes);

    /// <summary>
    /// A callback to be invoked after completing the purchase (both successfully or with an error).
    /// </summary>
    /// <param name="id">The item ID.</param>
    public delegate void PaymentCompletedCallback(string id);
}
