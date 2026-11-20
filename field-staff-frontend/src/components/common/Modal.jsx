
function Modal({ isOpen, onClose, title, children, size = "md" }) {
  if (!isOpen) return null;

  const sizeClasses = {
    sm: "max-w-sm",
    md: "max-w-md",
    lg: "max-w-lg",
    xl: "max-w-xl",
    "2xl": "max-w-5xl",
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">

      <div
        className={`w-full ${sizeClasses[size]} bg-white rounded-xl shadow-xl`}
      >
        {/* Modal Header */}
        <div className="flex items-center justify-between px-6 py-4 border-b border-slate-200">

          <h2 className="text-xl font-semibold text-slate-800">
            {title}
          </h2>

          <button
            type="button"
            onClick={onClose}
            className="text-slate-500 hover:text-slate-800 text-2xl"
          >
            &times;
          </button>

        </div>

        {/* Modal Body */}
        <div className="p-6">
          {children}
        </div>

      </div>
    </div>
  );
}

export default Modal;
