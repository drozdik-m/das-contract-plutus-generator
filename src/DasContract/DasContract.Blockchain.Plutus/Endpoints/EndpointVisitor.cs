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
        public EndpointVisitor(ContractStartEvent initialStartEvent)
        {
            InitialStartEvent = initialStartEvent;
        }

        Dictionary<string, bool> isFirstTxDictionary = new Dictionary<string, bool>();

        List<(string, PlutusFunctionSignature)> createdEndpoints = new List<(string, PlutusFunctionSignature)>();

        Stack<ContractProcess> processStack = new Stack<ContractProcess>();

        public ContractStartEvent InitialStartEvent { get; }        

        /// <summary>
        /// Checks if the element has the "first" flag
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        bool HasFirstFlag(INamable element) 
            => isFirstTxDictionary.ContainsKey(element.Name) && isFirstTxDictionary[element.Name];

        /// <summary>
        /// States that the element does not have the first flag
        /// </summary>
        /// <param name="element"></param>
        void ThisHasNotFirstFlag(INamable element)
        {
            if (isFirstTxDictionary.ContainsKey(element.Name))
                isFirstTxDictionary[element.Name] = false;
            else
                isFirstTxDictionary.Add(element.Name, false);
        }

        /// <summary>
        /// States that the element does have the first flag
        /// </summary>
        /// <param name="element"></param>
        void ThisHasFirstFlag(INamable element)
        {
            if (isFirstTxDictionary.ContainsKey(element.Name))
                isFirstTxDictionary[element.Name] = true;
            else
                isFirstTxDictionary.Add(element.Name, true);   
        }

        /// <summary>
        /// Sends flag of the source to the target
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        void SendFlagFurther(INamable source, INamable target)
        {
            //Send flag further
            if (HasFirstFlag(source))
                ThisHasFirstFlag(target);
            else
                ThisHasNotFirstFlag(target);
        }

        /// <summary>
        /// Sends flag of the source to the targets
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targets"></param>
        void SendFlagFurther(INamable source, IEnumerable<INamable> targets)
        {
            foreach(var target in targets)
                SendFlagFurther(source, target);
        }


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
        IEnumerable<IPlutusLine> CreateRedeemer(int indent, ContractUserActivity userActivity) => new IPlutusLine[]
           {
                new PlutusComment(indent, "Create redeemer"),
                new PlutusRawLine(indent, $"let redeemer = {userActivity.Name} form"),
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
            IEnumerable<IPlutusLine> timedOut, 
            IEnumerable<IPlutusLine> regular) => new IPlutusLine[]
           {
                new PlutusComment(indent, "Check timeout"),
                new PlutusRawLine(indent, $"datum <- onChainDatum client"),
                new PlutusRawLine(indent, $"now <- currentTime"),
                new PlutusRawLine(indent, $"let timeout = setNumberTimeout datum"),
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

            //Send flag further
            SendFlagFurther(element, element.Outgoing.Select(e => e.Target));

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

            //Send flag further
            SendFlagFurther(element, element.Outgoing);

            //Evaluate further elements
            return element.Outgoing.Accept(this);
        }

        public override IPlutusCode Visit(ContractStartEvent element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            //Check for initial start init
            if (InitialStartEvent == element)
                ThisHasFirstFlag(element);

            //Send flag further
            SendFlagFurther(element, element.Outgoing);

            //Evaluate further elements
            return element.Outgoing.Accept(this);
        }

        public override IPlutusCode Visit(ContractEndEvent element)
        {
            //Already visited
            if (!TryVisit(element) || processStack.Any())
                return PlutusCode.Empty;

            PlutusFunctionSignature signature;
            string parameter;

            var functionLines = Do(1).ToList();
            functionLines.AddRange(EndpointBegun(1, EndpointName(element)));

            //Is this first endpoint?
            if (HasFirstFlag(element))
            {
                signature = FinishContractEndpointSignature(isFirst: true);
                parameter = string.Empty;
                functionLines.AddRange(InitialClientSetup(1));
                functionLines.AddRange(ContractInit(1));

                ThisHasNotFirstFlag(element);
            }

            //This is not the first endpoint
            else
            {
                signature = FinishContractEndpointSignature(isFirst: false);
                functionLines.AddRange(ClientSetup(1));
                parameter = "threadToken";
            }

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
            SendFlagFurther(element, element.CalledProcess.StartEvent);
            var subprocessResult = element.CalledProcess.StartEvent.Accept(this);
            processStack.Pop();

            //Send flag further
            SendFlagFurther(element, element.Outgoing);

            //Evaluate further elements
            return element.Outgoing.Accept(this)
                .Append(subprocessResult);
        }

        public override IPlutusCode Visit(ContractUserActivity element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            var timeoutBoundary = element.BoundaryEvents.OfType<ContractBoundaryEvent>().FirstOrDefault();

            var functionLines = Do(1).ToList();
            functionLines.AddRange(EndpointBegun(1, EndpointName(element)));

            PlutusFunctionSignature signature;
            string parameter = string.Empty;

            //Is this first endpoint?
            if (HasFirstFlag(element))
            {
                signature = EndpointSignature(element, isFirst: true);
                parameter = "form";
                functionLines.AddRange(InitialClientSetup(1));
                functionLines.AddRange(TokenShare(1));
                functionLines.AddRange(ContractInit(1));

                ThisHasNotFirstFlag(element);
            }

            //This is not the first endpoint
            else
            {
                signature = EndpointSignature(element, isFirst: false);
                functionLines.AddRange(ClientSetup(1));
                parameter = "(form, threadToken)";
            }


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

                functionLines.AddRange(TimeoutCheck(1, timeoutCode, regularCode));
            }

            functionLines.AddRange(EndpointEnded(1, EndpointName(element)));


            //Calculate the result
            IPlutusCode result = new PlutusFunction(0, signature, new [] { parameter }, functionLines)
                .Prepend(signature);

            createdEndpoints.Add((element.Name.FirstCharToLowerCase(), signature));

            //Send flag further
            SendFlagFurther(element, element.Outgoing);
            if (!(timeoutBoundary is null))
                SendFlagFurther(element, timeoutBoundary);

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

            //Send flag further
            SendFlagFurther(element, element.Outgoing);

            //Evaluate further elements
            return element.Outgoing.Accept(this);
        }

        public override IPlutusCode Visit(ContractTimerBoundaryEvent element)
        {
            //Already visited
            if (!TryVisit(element))
                return PlutusCode.Empty;

            //Send flag further
            SendFlagFurther(element, element.TimeOutDirection);

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

        public static PlutusFunctionSignature TimedOutEndpointSignature { get; } = new PlutusFunctionSignature(0,
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

        public static PlutusFunctionSignature FinishContractEndpointSignature(bool isFirst)
        {
            IEnumerable<INamable> types = new List<INamable>();
            if (isFirst)
            {
                types = new INamable[]
                {
                     PlutusContractMonad.Type(
                         PlutusUnspecifiedDataType.Type("w"),
                         PlutusUnspecifiedDataType.Type("s"),
                         PlutusText.Type,
                         PlutusVoid.Type
                         )
                };
            }
            else
            {
                types = new INamable[]
                {
                     PlutusThreadToken.Type,
                     PlutusContractMonad.Type(
                         PlutusUnspecifiedDataType.Type("w"),
                         PlutusUnspecifiedDataType.Type("s"),
                         PlutusText.Type,
                         PlutusVoid.Type
                         )
                };
            }


           return new PlutusFunctionSignature(0,
               "finishContractEndpoint",
               types);
        }

        public static string EndpointName(INamable element)
            => element.Name.FirstCharToLowerCase() + "Endpoint";

        public static PlutusFunctionSignature EndpointSignature(ContractUserActivity userActivity, bool isFirst)
        {
            IEnumerable<INamable> types;
            if (isFirst)
            {
                types = new INamable[]
                {
                    PlutusUserActivityForm.Type(userActivity),
                    PlutusContractMonad.Type(
                        PlutusLast.Type(PlutusThreadToken.Type),
                        PlutusUnspecifiedDataType.Type("s"),
                        PlutusText.Type,
                        PlutusVoid.Type
                    )
                };
            }
            else
            {
                types = new INamable[]
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
                };
            }


            return new PlutusFunctionSignature(0, EndpointName(userActivity), types);
        }
            
    }
}
