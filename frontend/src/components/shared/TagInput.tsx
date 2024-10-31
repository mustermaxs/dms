import CreatableSelect from "react-select/creatable";

interface Tag {
  value: string;
  label: string;
}

interface TagInputProps {
  selectedTags: Tag[];
  onChange: (newTags: Tag[]) => void;
  availableTags?: Tag[];
  placeholder?: string;
  className?: string;
}

export const TagInput = ({ 
  selectedTags, 
  onChange, 
  availableTags = [], 
  placeholder = "Add or create tags...",
  className = ""
}) => {
  const customStyles = {
    multiValue: (styles: any) => ({
      ...styles,
      backgroundColor: '#DBEAFE', // bg-blue-100
      color: '#1E40AF', // text-blue-800
      fontSize: '0.9rem', // text-xs
      fontWeight: '600', // font-semibold
      padding: '0.3rem 0.4rem', // px-2.5 py-0.5
      borderRadius: '0.375rem', // rounded
      ':hover': {
        cursor: 'pointer',
      },
    }),
    multiValueLabel: (styles: any) => ({
      ...styles,
      color: '#1E40AF', // text-blue-800
      padding: 0,
    }),
    multiValueRemove: (styles: any) => ({
      ...styles,
      color: '#1E40AF', // text-blue-800
      marginLeft: '0.3rem',
      padding: '0.1rem',
      ':hover': {
        backgroundColor: '#BFDBFE',
      },
    }),
    control: (styles: any) => ({
      ...styles,
      fontSize: '0.9rem',
      borderRadius: '0.375rem', // rounded
      ':hover': {
        borderColor: 'black', // border-blue-800
        cursor: 'pointer',
      },
    }),
    menu: (styles: any) => ({
      ...styles,
      fontSize: '0.9rem',
    }),
  };

  return (
    <CreatableSelect
      options={availableTags}
      isMulti
      value={selectedTags}
      onChange={onChange}
      placeholder={placeholder}
      className={className}
      styles={customStyles}
    />
  );
};
