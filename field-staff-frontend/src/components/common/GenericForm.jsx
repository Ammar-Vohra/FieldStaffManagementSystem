
import { useEffect, useState } from "react";

function GenericForm({
    fields = [],
    initialData = {},
    saving = false,
    submitText = "Save",
    onSubmit,
    onCancel
}) {

    const [formData, setFormData] = useState(initialData);
    const [errors, setErrors] = useState({});


    // Synchronize form whenever create/edit data changes
    useEffect(() => {

        setFormData(initialData);
        setErrors({});

    }, [initialData]);


    const handleChange = (name, value) => {

        setFormData(prev => ({
            ...prev,
            [name]: value
        }));

        setErrors(prev => ({
            ...prev,
            [name]: "",
            general: ""
        }));

    };


    const validate = () => {

        const newErrors = {};

        fields.forEach(field => {

            const value = formData[field.name];

            if (field.required && !String(value ?? "").trim()) {

                newErrors[field.name] =
                    `${field.label} is required`;

            }

            if (
                field.minLength &&
                value &&
                String(value).length < field.minLength
            ) {

                newErrors[field.name] =
                    `${field.label} must be at least ${field.minLength} characters`;

            }

            if (
                field.maxLength &&
                value &&
                String(value).length > field.maxLength
            ) {

                newErrors[field.name] =
                    `${field.label} cannot exceed ${field.maxLength} characters`;

            }

            if (field.type === "email" && value) {

                const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

                if (!emailRegex.test(value)) {

                    newErrors[field.name] =
                        "Enter a valid email address";

                }

            }

        });

        setErrors(newErrors);

        return Object.keys(newErrors).length === 0;

    };


    const handleSubmit = (e) => {

        e.preventDefault();

        if (!validate()) return;

        onSubmit(formData, setErrors);

    };


    return (

        <form onSubmit={handleSubmit} className="space-y-4">

            {errors.general && (

                <p className="text-sm text-red-600">
                    {errors.general}
                </p>

            )}


            {fields.map(field => (

                <div key={field.name}>

                    <label className="block text-sm font-medium mb-1">
                        {field.label}
                        {field.required && (
                            <span className="text-red-500 ml-1">*</span>
                        )}
                    </label>


                    {field.type === "select" ? (

                        <select
                            value={formData[field.name] ?? ""}
                            onChange={e =>
                                handleChange(field.name, e.target.value)
                            }
                            disabled={saving || field.disabled}
                            className="w-full px-3 py-2 border border-slate-300 rounded-lg outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 disabled:bg-slate-100"
                        >

                            <option value="">
                                {field.placeholder || "Select an option"}
                            </option>

                            {field.options?.map(option => (

                                <option
                                    key={option.value}
                                    value={option.value}
                                >
                                    {option.label}
                                </option>

                            ))}

                        </select>

                    ) : field.type === "checkbox" ? (

                        <input
                            type="checkbox"
                            checked={Boolean(formData[field.name])}
                            onChange={e =>
                                handleChange(field.name, e.target.checked)
                            }
                            disabled={saving || field.disabled}
                            className="w-4 h-4 accent-blue-600"
                        />

                    ) : (

                        <input
                            type={field.type || "text"}
                            value={formData[field.name] ?? ""}
                            onChange={e =>
                                handleChange(field.name, e.target.value)
                            }
                            placeholder={field.placeholder || ""}
                            min={field.min}
                            max={field.max}
                            disabled={saving || field.disabled}
                            className="w-full px-3 py-2 border border-slate-300 rounded-lg outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 disabled:bg-slate-100"
                        />

                    )}


                    {errors[field.name] && (

                        <p className="text-sm text-red-600 mt-1">
                            {errors[field.name]}
                        </p>

                    )}

                </div>

            ))}


            <div className="flex justify-end gap-3 pt-3">

                <button
                    type="button"
                    onClick={onCancel}
                    disabled={saving}
                    className="px-4 py-2 border border-slate-300 rounded-lg hover:bg-slate-50 disabled:opacity-50"
                >
                    Cancel
                </button>


                <button
                    type="submit"
                    disabled={saving}
                    className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50"
                >
                    {saving ? "Saving..." : submitText}
                </button>

            </div>

        </form>

    );

}

export default GenericForm;
