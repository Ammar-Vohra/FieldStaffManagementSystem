import { useEffect, useRef, useState } from "react";
import ReactMarkdown from "react-markdown";
import remarkGfm from "remark-gfm";

import chatService from "../../services/chatService";

const roleQuestions = {
    FieldStaff: [
        "Show me my pending tasks",
        "Show me my completed tasks",
        "Show me my in progress tasks",
        "How many tasks do I have?",
    ],

    Supervisor: [
        "Show me today's tasks",
        "Which tasks are overdue?",
        "Show me staff workload",
        "Give me an operational summary",
    ],

    Admin: [
        "Give me an overall operational summary",
        "How many tasks are overdue?",
        "Show me staff workload",
        "Show me all service tasks",
    ],
};



function createMessage(role, content) {
    return {
        id: `${Date.now()}-${Math.random().toString(36).slice(2, 9)}`,
        role,
        content,
    };
}

function Chatbot({roleName}) {

    const suggestedQuestions = roleQuestions[roleName?.trim()] || [
    "Give me an operational summary",
    ];


    console.log("Chatbot received roleName:", roleName);
    console.log("Suggested questions:", suggestedQuestions);

    const [isOpen, setIsOpen] = useState(false);
    const [messages, setMessages] = useState([]);
    const [input, setInput] = useState("");
    const [loading, setLoading] = useState(false);

    const messagesEndRef = useRef(null);
    const inputRef = useRef(null);

    useEffect(() => {
        if (isOpen) {
            messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
        }
    }, [messages, loading, isOpen]);

    useEffect(() => {
        if (!isOpen) return;

        inputRef.current?.focus();

        const handleKeyDown = (event) => {
            if (event.key === "Escape") {
                setIsOpen(false);
            }
        };

        window.addEventListener("keydown", handleKeyDown);
        return () => window.removeEventListener("keydown", handleKeyDown);
    }, [isOpen]);


    const sendMessage = async (rawMessage) => {
    const trimmed = rawMessage.trim();

    if (!trimmed || loading) return;

    const userMessage = createMessage("user", trimmed);

    // Keep previous conversation messages only.
    const history = messages
        .filter(message =>
            message.role === "user" ||
            message.role === "assistant"
        )
        .map(({ role, content }) => ({
            role,
            content
        }));

    setMessages(prev => [...prev, userMessage]);
    setInput("");
    setLoading(true);

    try {
        const response = await chatService.sendMessage(
            trimmed,
            history
        );

        const replyText =
            response?.reply ??
            response?.data?.reply ??
            "I didn't receive a response. Please try again.";

        setMessages(prev => [
            ...prev,
            createMessage("assistant", replyText)
        ]);
    } catch (error) {
        console.error("Chat error:", error);

        setMessages(prev => [
            ...prev,
            createMessage(
                "assistant",
                "Something went wrong. Please try again."
            )
        ]);
    } finally {
        setLoading(false);
    }
};


    const handleSendMessage = (e) => {
        e.preventDefault();
        sendMessage(input);
    };

    const handleClearChat = () => {
        if (loading) return;
        setMessages([]);
        setInput("");
    };

    return (
        <>
            {/* Floating Assistant Button */}
            <button
                type="button"
                onClick={() => setIsOpen((prev) => !prev)}
                className="fixed bottom-6 right-6 z-50 flex h-14 w-14
                    items-center justify-center rounded-full bg-blue-600
                    text-white shadow-lg transition hover:scale-105
                    hover:bg-blue-700 focus:outline-none focus:ring-2
                    focus:ring-blue-500 focus:ring-offset-2"
                title="AI Assistant"
                aria-label="Toggle AI Assistant"
                aria-expanded={isOpen}
            >
                {isOpen ? (
                    <span className="text-2xl leading-none" aria-hidden="true">
                        &times;
                    </span>
                ) : (
                    <svg
                        xmlns="http://www.w3.org/2000/svg"
                        className="h-7 w-7"
                        fill="none"
                        viewBox="0 0 24 24"
                        stroke="currentColor"
                        strokeWidth={2}
                        aria-hidden="true"
                    >
                        <path
                            strokeLinecap="round"
                            strokeLinejoin="round"
                            d="M8 10h.01M12 10h.01M16 10h.01M21 12c0 4.418-4.03 8-9 8a10.2 10.2 0 01-4-.8L3 20l1.2-3.6A7.5 7.5 0 013 12c0-4.418 4.03-8 9-8s9 3.582 9 8z"
                        />
                    </svg>
                )}
            </button>

            {/* Chat Window */}
            {isOpen && (
                <div
                    role="dialog"
                    aria-label="AI Operational Assistant"
                    className="fixed bottom-24 right-6 z-50 flex
                        h-[550px] w-[calc(100vw-2rem)] max-w-[400px]
                        flex-col overflow-hidden rounded-xl border
                        border-gray-200 bg-white shadow-2xl"
                >
                    {/* Header */}
                    <div className="flex items-center justify-between
                        bg-blue-600 px-5 py-4 text-white">
                        <div>
                            <h2 className="font-semibold">
                                AI Operational Assistant
                            </h2>
                            <p className="text-xs text-blue-100">
                                Your field operations assistant
                            </p>
                        </div>

                        <div className="flex items-center gap-2">
                            <button
                                type="button"
                                onClick={handleClearChat}
                                disabled={messages.length === 0 || loading}
                                title="Clear conversation"
                                className="rounded px-2.5 py-1.5 text-sm transition
                                    hover:bg-blue-700 disabled:cursor-not-allowed
                                    disabled:opacity-40"
                            >
                                Clear
                            </button>

                            <button
                                type="button"
                                onClick={() => setIsOpen(false)}
                                className="rounded px-2 py-1 text-xl leading-none
                                    transition hover:bg-blue-700"
                                aria-label="Close chatbot"
                            >
                                &times;
                            </button>
                        </div>
                    </div>

                    {/* Messages */}
                    <div className="flex-1 space-y-4 overflow-y-auto
                        bg-gray-50 p-4">
                        {/* Welcome Screen */}
                        {messages.length === 0 && (
                            <div className="flex min-h-full flex-col
                                justify-center gap-4">
                                <div className="text-center">
                                    <div className="mb-3 text-4xl">
                                        <span role="img" aria-label="Robot">
                                            🤖
                                        </span>
                                    </div>

                                    <h3 className="font-semibold text-gray-800">
                                        How can I help you?
                                    </h3>

                                    <p className="mt-1 text-sm text-gray-500">
                                        Ask me about your service operations.
                                    </p>
                                </div>

                                {/* Suggested Questions */}
                                <div className="space-y-2">
                                    <p className="text-xs font-semibold
                                        uppercase tracking-wide text-gray-400">
                                        Suggested questions
                                    </p>

                                    {suggestedQuestions.map((question) => (
                                        <button
                                            key={question}
                                            type="button"
                                            onClick={() => sendMessage(question)}
                                            disabled={loading}
                                            className="w-full rounded-lg border
                                                border-gray-200 bg-white px-3 py-3
                                                text-left text-sm text-gray-700
                                                transition hover:border-blue-400
                                                hover:bg-blue-50 disabled:cursor-not-allowed
                                                disabled:opacity-50"
                                        >
                                            {question}
                                        </button>
                                    ))}
                                </div>
                            </div>
                        )}

                        {/* Conversation */}
                        {messages.map((message) => (
                            <div
                                key={message.id}
                                className={`flex ${
                                    message.role === "user"
                                        ? "justify-end"
                                        : "justify-start"
                                }`}
                            >
                                <div
                                    className={`max-w-[90%] rounded-xl px-3 py-2
                                        text-sm ${
                                        message.role === "user"
                                            ? "bg-blue-600 text-white"
                                            : "border border-gray-200 bg-white text-gray-800 shadow-sm"
                                    }`}
                                >
                                    {message.role === "assistant" ? (
                                        <div className="prose prose-sm max-w-none
                                            prose-table:border-collapse
                                            prose-th:border prose-th:border-gray-300
                                            prose-th:bg-gray-100 prose-th:p-2
                                            prose-td:border prose-td:border-gray-300
                                            prose-td:p-2">
                                            <ReactMarkdown
                                                remarkPlugins={[remarkGfm]}
                                            >
                                                {message.content}
                                            </ReactMarkdown>
                                        </div>
                                    ) : (
                                        <p className="whitespace-pre-wrap">
                                            {message.content}
                                        </p>
                                    )}
                                </div>
                            </div>
                        ))}

                        {/* Animated Typing Indicator */}
                        {loading && (
                            <div className="flex justify-start" aria-live="polite">
                                <div className="flex items-center gap-1
                                    rounded-xl border border-gray-200
                                    bg-white px-4 py-3 shadow-sm">
                                    <span className="h-2 w-2 animate-bounce
                                        rounded-full bg-blue-500"></span>
                                    <span className="h-2 w-2 animate-bounce
                                        rounded-full bg-blue-500
                                        [animation-delay:150ms]"></span>
                                    <span className="h-2 w-2 animate-bounce
                                        rounded-full bg-blue-500
                                        [animation-delay:300ms]"></span>
                                </div>
                            </div>
                        )}

                        <div ref={messagesEndRef} />
                    </div>

                    {/* Message Input */}
                    <form
                        onSubmit={handleSendMessage}
                        className="flex items-center gap-2 border-t
                            border-gray-200 bg-white p-3"
                    >
                        <input
                            ref={inputRef}
                            type="text"
                            value={input}
                            onChange={(e) => setInput(e.target.value)}
                            placeholder="Ask something..."
                            disabled={loading}
                            aria-label="Message input"
                            className="min-w-0 flex-1 rounded-lg border
                                border-gray-300 px-3 py-2 text-sm
                                focus:outline-none focus:ring-2
                                focus:ring-blue-500 disabled:bg-gray-100"
                        />

                        <button
                            type="submit"
                            disabled={loading || !input.trim()}
                            className="rounded-lg bg-blue-600 px-4 py-2
                                text-sm font-medium text-white
                                transition hover:bg-blue-700
                                disabled:cursor-not-allowed
                                disabled:opacity-50"
                        >
                            Send
                        </button>
                    </form>
                </div>
            )}
        </>
    );
}

export default Chatbot; 