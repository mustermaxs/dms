
export default function Label({ title }: { title: string }) {
    return (
        <label className="block text-sm font-medium text-gray-700">
            {title}
        </label>
    )
}
