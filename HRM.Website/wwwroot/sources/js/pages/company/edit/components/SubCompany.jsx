import React, { useState, useRef, useCallback } from 'react';
import toastr from 'toastr';
import { toastOptions } from '../../../../shared/common.functions';

// Mock API call to check if tag exists in system
const checkTagExists = async (tag) => {
  return new Promise((resolve) => {
    setTimeout(() => {
      const validTags = ['apple', 'banana', 'grape']; // Example valid tags
      const existsInSystem = validTags.includes(tag.toLowerCase());
      resolve(existsInSystem);
    }, 500);
  });
};

const SubCompany = ({ companyId, initialTags = [], onTagsChange }) => {
  const [tags, setTags] = useState(initialTags);
  const [inputValue, setInputValue] = useState('');
  const [editingIndex, setEditingIndex] = useState(null);

  const [isLoading, setIsLoading] = useState(false);
  const inputRef = useRef(null);

  const clearInput = useCallback(() => {
    setInputValue('');
  }, []);

  const resetEditing = useCallback(() => {
    setEditingIndex(null);
    clearInput();
  }, [clearInput]);

  const handleAddTag = useCallback(async (trimmedValue) => {
    if (!trimmedValue) return;

    setIsLoading(true);

    try {
      const exists = await checkTagExists(trimmedValue);

      if (exists) {
        toastr.warning(`Tag "${trimmedValue}" already exists in the system.`, 'Warning', toastOptions());
        return;
      }

      if (tags.includes(trimmedValue)) {
        toastr.warning(`Tag "${trimmedValue}" already exists in the list.`, 'Warning', toastOptions());
        return;
      }

      const updatedTags = [...tags, trimmedValue];
      setTags(updatedTags);
    //   onTagsChange?.(updatedTags);
      clearInput();
      toastr.success(`Tag "${trimmedValue}" added successfully.`, 'Success', toastOptions());
    } catch (err) {
      toastr.error('An error occurred while checking the tag. Please try again.', 'Error', toastOptions());
    } finally {
      setIsLoading(false);
    }
  }, [tags, onTagsChange, clearInput]);

  const handleEditTag = useCallback(async (trimmedValue) => {
    if (!trimmedValue) {
      resetEditing();
      return;
    }

    setIsLoading(true);

    try {
      const exists = await checkTagExists(trimmedValue);

      if (exists) {
        toastr.warning(`"${trimmedValue}" already exists!`, 'Warning', toastOptions());
        return;
      }

      if (tags.some((tag, index) => index !== editingIndex && tag === trimmedValue)) {
        toastr.warning(`"${trimmedValue}" already exists!`, 'Warning', toastOptions());
        return;
      }

      const updatedTags = [...tags];
      updatedTags[editingIndex] = trimmedValue;
      setTags(updatedTags);
    //   onTagsChange?.(updatedTags);
      resetEditing();
      toastr.success(`Tag "${trimmedValue}" updated successfully.`, 'Success', toastOptions());
    } catch (err) {
      toastr.error('An error occurred while updating the tag. Please try again.', 'Error', toastOptions());
    } finally {
      setIsLoading(false);
    }
  }, [tags, editingIndex, onTagsChange, resetEditing]);

  const handleKeyDown = useCallback(async (e) => {
    if (e.key === 'Enter' && inputValue.trim() !== '' && !isLoading) {
      e.preventDefault();
      const trimmed = inputValue.trim();

      if (editingIndex !== null) {
        await handleEditTag(trimmed);
      } else {
        await handleAddTag(trimmed);
      }
    } else if (e.key === 'Escape') {
      resetEditing();
    }
  }, [inputValue, editingIndex, isLoading, handleAddTag, handleEditTag, resetEditing]);

  const handleInputChange = useCallback((e) => {
    setInputValue(e.target.value);
  }, []);

  const handleEdit = useCallback((index) => {
    setEditingIndex(index);
    setInputValue(tags[index]);
    // setTimeout(() => inputRef.current?.focus(), 0);
  }, [tags]);

  const handleRemove = useCallback((index) => {
    const tagToRemove = tags[index];
    const updatedTags = tags.filter((_, i) => i !== index);
    setTags(updatedTags);
    // onTagsChange?.(updatedTags);
    
    if (editingIndex === index) {
      resetEditing();
    }
    
    toastr.success(`Tag "${tagToRemove}" removed successfully.`, 'Success', toastOptions());
  }, [tags, editingIndex, onTagsChange, resetEditing]);

  const handleBlur = useCallback(() => {
    if (editingIndex !== null && inputValue.trim() !== '') {
      handleEditTag(inputValue.trim());
    } else {
      resetEditing();
    }
  }, [editingIndex, inputValue, handleEditTag, resetEditing]);

  const renderTag = useCallback((tag, index) => (
    <div
      key={index}
      className={`tag-item ${editingIndex === index ? 'editing' : ''}`}
    >
      <span 
        onClick={() => handleEdit(index)} 
        className={`tag-text ${editingIndex === index ? 'editing' : ''}`}
      >
        {editingIndex === index ? (
          <input
            ref={inputRef}
            id={`tag-edit-${index}`}
            name={`tag-edit-${index}`}
            value={inputValue}
            onChange={handleInputChange}
            onKeyDown={handleKeyDown}
            onBlur={handleBlur}
            className="tag-edit-input"
            disabled={isLoading}
          />
        ) : (
          tag
        )}
      </span>
      <button
        onClick={() => handleRemove(index)}
        disabled={isLoading}
        className="tag-remove-btn"
        title="Remove tag"
      >
        <span className="tag-remove-btn-text">✕</span>
      </button>
    </div>
  ), [editingIndex, inputValue, isLoading, handleEdit, handleInputChange, handleKeyDown, handleBlur, handleRemove]);

  return (
    <div className="sub-company-container">
      <div className="tag-input-container">
        {tags.map(renderTag)}
        {editingIndex === null && (
        <div className="input-field-wrapper">
            <input
            ref={inputRef}
            id="sub-company-input"
            name="sub-company-input"
            value={inputValue}
            onChange={handleInputChange}
            onKeyDown={handleKeyDown}
            placeholder= "Enter company name and press Enter"
            disabled={isLoading}
            className="tag-input-field"
            />
        </div>
        )}
      </div>
    </div>
  );
};

export default SubCompany;
