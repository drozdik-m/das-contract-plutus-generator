using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using DasContract.Blockchain.Plutus.Code.Types.Determined;
using DasContract.Blockchain.Plutus.Code.Types.Premade;
using DasContract.Blockchain.Plutus.Code.Types.Temporary;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using DasContract.Blockchain.Plutus.Data.Processes.Process;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Activities;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Events;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Gateways;
using DasContract.Blockchain.Plutus.Data.Processes.Process.MultiInstances;
using DasContract.String.Utils;

namespace DasContract.Blockchain.Plutus.Transitions
{
    public class EndpointVisitor : RecursiveElementVisitor
    {

        List<(string, PlutusFunctionSignature)> createdEndpoints = new List<(string, PlutusFunctionSignature)>();

        Stack<ContractProcess> processStack = new Stack<ContractProcess>();

        #region snippets
        /// <summary>
        /// Returns the do line
        /// </summary>
        /// <param name="indent"></param>
        /// <returns></returns>
        IEnumerable<IPlutusLine> Do(int indent)
            => new IPlutusLine[]
            {
                new PlutusRawLine(indent, "do"),
                PlutusLine.Empty
            };

        /// <summary>
        /// Lines that initially setup the contract client
        /// </summary>
        /// <returns></returns>
        IEnumerable<IPlutusLine> InitialClientSetup(int indent) => new IPlutusLine[]
            {
                new PlutusComment(indent, "Initial client setup"),
                new PlutusRawLine(indent, "contractParam <- initContractParam"),
                new PlutusRawLine(indent, "let threadToken = cpToken contractParam"),
                new PlutusRawLine(indent, "let client = contractClient contractParam"),
                new PlutusRawLine(indent, "logInfo @String \"--- client created\""),
                PlutusLine.Empty,
            };

        /// <summary>
        /// Lines that setup the contract client
        /// </summary>
        /// <returns></returns>
        IEnumerable<IPlutusLine> ClientSetup(int indent) => new IPlutusLine[]
            {
                new PlutusComment(indent, "Client setup"),
                new PlutusRawLine(indent, "contractParam <- createContractParam threadToken"),
                new PlutusRawLine(indent, "let client = contractClient contractParam"),
                new PlutusRawLine(indent, "logInfo @String \"--- client created\""),
                PlutusLine.Empty,
            };

        /// <summary>
        /// Lines that share the thread token
        /// </summary>
        /// <returns></returns>
        IEnumerable<IPlutusLine> TokenShare(int indent) => new IPlutusLine[]
           {
                new PlutusComment(indent, "Token share"),
                new PlutusRawLine(indent, "tell $ Last $ Just threadToken"),
                new PlutusRawLine(indent, "logInfo @String \"--- token shared\""),
                PlutusLine.Empty,
           };

        /// <summary>
        /// Lines that initiate the contract
        /// </summary>
        /// <returns></returns>
        IEnumerable<IPlutusLine> ContractInit(int indent) => new IPlutusLine[]
           {
                new PlutusComment(indent, "Initialization"),
                new PlutusRawLine(indent, "void $ mapErr $ runInitialise client initialDatum (lovelaceValueOf 0)"),
                new PlutusRawLine(indent, "logInfo @String \"CONTRACT STARTED\""),
                new PlutusRawLine(indent, "logInfo @String \"--- contract initialized\""),
                PlutusLine.Empty,
           };

        /// <summary>
        /// Lines that create the cedeemer
        /// </summary>
        /// <returns></returns>
        IEnumerable<IPlutusLine> CreateRedeemer(int indent, string redeemerName) => new IPlutusLine[]
           {
                new PlutusComment(indent, "Create redeemer"),
                new PlutusRawLine(indent, $"let redeemer = {redeemerName}"),
                PlutusLine.Empty,
           };

        /// <summary>
        /// Lines that create the cedeemer
        /// </summary>
        /// <returns></returns>
        IEnumerable<IPlutusLine> CreateRedeemer(int indent, ContractUserActivity userActivity)
            => CreateRedeemer(indent, $"{PlutusUserActivityRedeemer.Type(userActivity).Name} form");

        /// <summary>
        /// Lines that validate the form
        /// </summary>
        /// <returns></returns>
        IEnumerable<IPlutusLine> ValidateForm(int indent) => new IPlutusLine[]
           {
                new PlutusComment(indent, "Validate form"),
                new PlutusRawLine(indent, $"validateInputForm threadToken client redeemer"),
                PlutusLine.Empty,
           };

        /// <summary>
        /// Lines that transition the contract state
        /// </summary>
        /// <returns></returns>
        IEnumerable<IPlutusLine> StateTransition(int indent, bool logState = true)
        {
            var result = new List<IPlutusLine>
            {
                new PlutusComment(indent, "State transition"),
                new PlutusRawLine(indent, $"void $ mapErr $ runStep client redeemer")
            };

            if (logState)
                result.Add(new PlutusRawLine(indent, $"logOnChainDatum client"));

            result.Add(new PlutusRawLine(indent, $"logInfo @String \"--- transition finished\""));
            result.Add(PlutusLine.Empty);

            return result;
        }

        /// <summary>
        /// Lines that begin the endpoint
        /// </summary>
        /// <returns></returns>
        IEnumerable<IPlutusLine> EndpointBegun(int indent, string endpointName) => new IPlutusLine[]
            {
                new PlutusRawLine(indent, $"logInfo @String \"{endpointName} called\""),
                PlutusLine.Empty,
            };

        /// <summary>
        /// Lines that end the endpoint
        /// </summary>
        /// <returns></returns>
        IEnumerable<IPlutusLine> EndpointEnded(int indent, string endpointName) => new IPlutusLine[]
            {
                new PlutusRawLine(indent, $"logInfo @String \"{endpointName} ended\""),
                PlutusLine.Empty,
            };

        /// <summary>
        /// Lines that clean the timed out state
        /// </summary>
        /// <returns></returns>
        IEnumerable<IPlutusLine> TimeoutCleaning(int indent) => new IPlutusLine[]
           {
                new PlutusComment(indent, "Timeout  cleaning"),
                new PlutusRawLine(indent, $"logInfo @String \"--- activity timed out, cleaning...\""),
                new PlutusRawLine(indent, $"{TimedOutEndpointSignature.Name} threadToken"),
                PlutusLine.Empty,
           };

        /// <summary>
        /// Lines that clean the timed out state
        /// </summary>
        /// <returns></returns>
        IEnumerable<IPlutusLine> TimeoutCheck(int indent, 
            string timeout,
            IEnumerable<IPlutusLine> timedOut, 
            IEnumerable<IPlutusLine> regular) => new IPlutusLine[]
           {
                new PlutusComment(indent, "Check timeout"),
                new PlutusRawLine(indent, $"datum <- onChainDatum client"),
                new PlutusRawLine(indent, $"now <- currentTime"),
                new PlutusRawLine(indent, $"let timeout = {timeout}"),
                new PlutusRawLine(indent, $"if now > timeout then do"),
                PlutusLine.Empty,
           }
           .Concat(timedOut)
           .Concat(new IPlutusLine[]
           {
               new PlutusRawLine(indent, $"else do"),
               PlutusLine.Empty,
           })
           .Concat(regular);
        #endregion

        #region elementEndpoints
        public override IPlutusCode Visit(ContractExclusiveGateway element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            //Evaluate further elements
            IPlutusCode result = PlutusCode.Empty;
            foreach (var target in element.Outgoing)
                result = result.Append(target.Target.Accept(this));

            return result;
        }

        public override IPlutusCode Visit(ContractMergingExclusiveGateway element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            //Evaluate further elements
            return element.Outgoing.Accept(this);
        }

        public override IPlutusCode Visit(ContractStartEvent element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            //Evaluate further elements
            return element.Outgoing.Accept(this);
        }

        public override IPlutusCode Visit(ContractEndEvent element)
        {
            //Already visited
            if (!TryVisit(element) || processStack.Any())
                return PlutusCode.Empty;

            var signature = FinishContractEndpointSignature;

            var functionLines = Do(1).ToList();
            functionLines.AddRange(EndpointBegun(1, signature.Name));

            
            functionLines.AddRange(ClientSetup(1));
            var parameter = "threadToken";

            functionLines.AddRange(CreateRedeemer(1, PlutusContractFinishedRedeemer.Type.Name));
            functionLines.AddRange(StateTransition(1, logState: false));
            functionLines.AddRange(EndpointEnded(1, signature.Name));
            functionLines.Add(new PlutusRawLine(1, "logInfo @String \"CONTRACT ENDED\""));
            functionLines.Add(PlutusLine.Empty);

            var function = new PlutusFunction(0, 
                signature,
                string.IsNullOrWhiteSpace(parameter) ? Array.Empty<string>() : new string[] { parameter }, 
                functionLines);

            createdEndpoints.Add(("finishContract", signature));

            return function.Prepend(signature);
        }

        public override IPlutusCode Visit(ContractCallActivity element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            //Go deepah!
            processStack.Push(element.CalledProcess);
            var subprocessResult = element.CalledProcess.StartEvent.Accept(this);
            processStack.Pop();

            //Evaluate further elements
            return element.Outgoing.Accept(this)
                .Append(subprocessResult);
        }

        public override IPlutusCode Visit(ContractUserActivity element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            var timeoutBoundary = element.BoundaryEvents.OfType<ContractTimerBoundaryEvent>().FirstOrDefault();

            var functionLines = Do(1).ToList();
            functionLines.AddRange(EndpointBegun(1, EndpointName(element)));

            PlutusFunctionSignature signature;

            signature = EndpointSignature(element);
            functionLines.AddRange(ClientSetup(1));
            var parameter = "(form, threadToken)";

            //No timer
            if (timeoutBoundary is null)
            {
                functionLines.AddRange(CreateRedeemer(1, element));
                functionLines.AddRange(ValidateForm(1));
                functionLines.AddRange(StateTransition(1));
            }

            //There is a timer
            else
            {
                IEnumerable<IPlutusLine> timeoutCode = TimeoutCleaning(2);
                IEnumerable<IPlutusLine> regularCode = CreateRedeemer(2, element)
                    .Concat(ValidateForm(2))
                    .Concat(StateTransition(2));

                functionLines.AddRange(TimeoutCheck(1, timeoutBoundary.TimerDefinition, timeoutCode, regularCode));
            }

            functionLines.AddRange(EndpointEnded(1, EndpointName(element)));


            //Calculate the result
            IPlutusCode result = new PlutusFunction(0, signature, new [] { parameter }, functionLines)
                .Prepend(signature);

            createdEndpoints.Add((element.Name.FirstCharToLowerCase(), signature));

            //Evaluate further elements
            result = result.Append(element.Outgoing.Accept(this));
            if (!(timeoutBoundary is null))
                result = result.Append(timeoutBoundary.Accept(this));
            return result;
        }

        public override IPlutusCode Visit(ContractScriptActivity element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            //Evaluate further elements
            return element.Outgoing.Accept(this);
        }

        public override IPlutusCode Visit(ContractTimerBoundaryEvent element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            //Evaluate further elements
            return element.TimeOutDirection.Accept(this);
        }
        #endregion

        #region extraEndpoints
        /// <summary>
        /// Creates the timeout endpoint
        /// </summary>
        /// <returns></returns>
        public IPlutusCode ContinueTimeoutActivityEndpoint()
        {
            var signature = TimedOutEndpointSignature;

            var code = Do(1).ToList();
            code.AddRange(EndpointBegun(1, signature.Name));
            code.AddRange(ClientSetup(1));
            code.AddRange(CreateRedeemer(1, PlutusTimeoutRedeemer.Type.Name));
            code.AddRange(StateTransition(1));
            code.AddRange(EndpointEnded(1, signature.Name));

            var function = new PlutusFunction(0, signature, new string[]
            {
                "threadToken"
            }, code);

            createdEndpoints.Add(("continueTimedoutActivity", signature));

            return function.Prepend(signature);
        }

        /// <summary>
        /// Creates the initialization endpoint
        /// </summary>
        /// <returns></returns>
        public IPlutusCode InitializeContractEndpoint()
        {
            var signature = InitializeEndpointSignature;

            var code = Do(1).ToList();

            code.AddRange(EndpointBegun(1, signature.Name));
            code.AddRange(InitialClientSetup(1));
            code.AddRange(TokenShare(1));
            code.AddRange(ContractInit(1));
            code.AddRange(EndpointEnded(1, signature.Name));

            var function = new PlutusFunction(0, signature, Array.Empty<string>(), code);

            createdEndpoints.Add(("initializeContract", signature));

            return function.Prepend(signature);
        }

        #endregion

        #region schemaAndEndpoints

        /// <summary>
        /// Takes all previously created endpoints and makes a schema out of them
        /// </summary>
        /// <returns></returns>
        public IPlutusCode MakeSchema()
        {
            IPlutusCode result = PlutusCode.Empty;

            result = result.Append(new PlutusRawLine(0, $"type {PlutusContractSchema.Type.Name} ="));
            var first = true;
            foreach(var endpInfo in createdEndpoints)
            {
                var name = endpInfo.Item1;
                var signature = endpInfo.Item2;

                var param = "()";
                if (signature.Types.Count() > 1)
                    param = signature.Types.First().Name;

                if (first)
                {
                    result = result.Append(new PlutusRawLine(1, $"    Endpoint \"{name}\" {param}"));
                    first = false;
                }
                else
                    result = result.Append(new PlutusRawLine(1, $".\\/ Endpoint \"{name}\" {param}"));
            }

            return result;
        }

        /// <summary>
        /// Takes all previously created endpoints and make contract endpoints
        /// </summary>
        /// <returns></returns>
        public IPlutusCode MakeEndpoints()
        {
            List<IPlutusLine> result = new List<IPlutusLine>();
            List<IPlutusLine> endpointDefinitions = new List<IPlutusLine>();

            result.Add(new PlutusRawLine(1, $"awaitPromise ("));
            var first = true;
            foreach (var endpInfo in createdEndpoints)
            {
                var name = endpInfo.Item1;
                var signature = endpInfo.Item2;

                if (first)
                {
                    result.Add(new PlutusRawLine(2, $"         {signature.Name}'"));
                    first = false;
                }
                else
                    result.Add(new PlutusRawLine(2, $"`select` {signature.Name}'"));

                var namePrefix = string.Empty;
                if (signature.Types.Count() == 1)
                    namePrefix = "$ const ";

                endpointDefinitions.Add(
                            new PlutusRawLine(2, $"{signature.Name}' = endpoint @\"{name}\" {namePrefix}{signature.Name}")
                        );

            }
            result.Add(new PlutusRawLine(1, $") >> endpoints"));

            var endpointsFunction = new PlutusFunction(
                0,
                EndpointsSignature,
                Array.Empty<string>(),
                result
                    .Append(new PlutusRawLine(1, "where"))
                    .Concat(endpointDefinitions));

            return endpointsFunction
                .Prepend(EndpointsSignature);
        }


        #endregion

        public static PlutusFunctionSignature EndpointsSignature { get; } = new PlutusFunctionSignature(0,
            "endpoints",
            new INamable[]
            {
                PlutusContractMonad.Type(
                        PlutusLast.Type(PlutusThreadToken.Type),
                        PlutusContractSchema.Type,
                        PlutusText.Type,
                        PlutusVoid.Type
                    )
            });

        public static PlutusFunctionSignature TimedOutEndpointSignature =>
            new PlutusFunctionSignature(0,
               "continueTimedoutActivityEndpoint",
               new INamable[]
                {
                    PlutusThreadToken.Type,
                    PlutusContractMonad.Type(
                        PlutusUnspecifiedDataType.Type("w"),
                        PlutusUnspecifiedDataType.Type("s"),
                        PlutusText.Type,
                        PlutusVoid.Type
                        )
                });

        public static PlutusFunctionSignature InitializeEndpointSignature =>
            new PlutusFunctionSignature(0,
               "initializeContractEndpoint",
               new INamable[]
                {
                    PlutusContractMonad.Type(
                        PlutusLast.Type(PlutusThreadToken.Type),
                        PlutusUnspecifiedDataType.Type("s"),
                        PlutusText.Type,
                        PlutusVoid.Type
                        )
                });

        public static PlutusFunctionSignature FinishContractEndpointSignature => 
            new PlutusFunctionSignature(0,
               "finishContractEndpoint",
               new INamable[]
                {
                     PlutusThreadToken.Type,
                     PlutusContractMonad.Type(
                         PlutusUnspecifiedDataType.Type("w"),
                         PlutusUnspecifiedDataType.Type("s"),
                         PlutusText.Type,
                         PlutusVoid.Type
                         )
                });

        public static string EndpointName(INamable element)
            => element.Name.FirstCharToLowerCase() + "Endpoint";

        public static PlutusFunctionSignature EndpointSignature(ContractUserActivity userActivity)
        {
            return new PlutusFunctionSignature(0, EndpointName(userActivity), new INamable[]
                {
                    PlutusTuple.Type(
                            PlutusUserActivityForm.Type(userActivity),
                            PlutusThreadToken.Type
                            ),
                    PlutusContractMonad.Type(
                        PlutusUnspecifiedDataType.Type("w"),
                        PlutusUnspecifiedDataType.Type("s"),
                        PlutusText.Type,
                        PlutusVoid.Type
                    )
                });
        }
            
    }
}

