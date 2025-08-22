import React, { useState, useRef, useCallback } from 'react';
import toastr from 'toastr';
import { toastOptions } from '../../../../shared/common.functions';

const checkTagExists = (tag, validTags = [], excludeCompanyId = null) => {
  return new Promise(resolve => {
      const existsInSystem = validTags.some(validTag => 
        validTag.name && 
        validTag.name.toLowerCase() === tag.toLowerCase() &&
        (!excludeCompanyId || validTag.companyId !== excludeCompanyId)
      );
      resolve(existsInSystem);
  });
};

const SubCompany = ({ companyId, initialTags = [], onTagsChange, validTags = [] }) => {
  const initialTagsWithAction = initialTags.map(tag => ({
    ...tag,
    actionType: tag.actionType || 'none'
  }));
  
  const [tags, setTags] = useState(initialTagsWithAction);
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

  const handleAddTag = useCallback((trimmedValue) => {
    if (!trimmedValue) {
      return;
    }

    setIsLoading(true);

    checkTagExists(trimmedValue, validTags, null).then(exists => {
      if (exists) {
        toastr.warning(`Sub Company "${trimmedValue}" has already exists!`,'', toastOptions());
        setIsLoading(false);
        return;
      }

      if (tags.some(tag => tag.name.toLowerCase() === trimmedValue.toLowerCase())) {
        toastr.warning(`Sub company "${trimmedValue}" has already exists!`, '', toastOptions());
        setIsLoading(false);
        return;
      }

      const updatedTags = [...tags, {
        subCompanyId: 0,
        companyId: companyId,
        name: trimmedValue,
        actionType: 'insert'
      }];
      setTags(updatedTags);
      if (onTagsChange) {
        onTagsChange(updatedTags);
      }
      clearInput();
      setIsLoading(false);
    }).catch(() => {
      setIsLoading(false);
    });
  }, [tags, onTagsChange, clearInput, validTags, companyId]);

  const handleEditTag = useCallback((trimmedValue) => {
    if (!trimmedValue) {
      resetEditing();
      return;
    }

    setIsLoading(true);

    checkTagExists(trimmedValue, validTags, companyId).then(exists => {
      if (exists) {
        toastr.warning(`"${trimmedValue}" has already exists!`, '', toastOptions());
        setIsLoading(false);
        return;
      }

      if (tags.some((tag, index) => index !== editingIndex && tag.name.toLowerCase() === trimmedValue.toLowerCase())) {
        toastr.warning(`"${trimmedValue}" has already exists!`, '', toastOptions());
        setIsLoading(false);
        return;
      }

      const updatedTags = [...tags];
      const currentTag = updatedTags[editingIndex];
      const newActionType = currentTag.actionType === 'insert' ? 'insert' : 'update';

      updatedTags[editingIndex] = {
        ...currentTag,
        name: trimmedValue,
        actionType: newActionType
      };
      setTags(updatedTags);
      if (onTagsChange) {
        onTagsChange(updatedTags);
      }
      resetEditing();
      setIsLoading(false);
    }).catch(() => {
      setIsLoading(false);
    });
  }, [tags, editingIndex, onTagsChange, resetEditing, validTags, companyId]);

  const handleKeyDown = useCallback((e) => {
    if (e.key === 'Enter' && inputValue.trim() !== '' && !isLoading) {
      e.preventDefault();
      const trimmed = inputValue.trim();

      if (editingIndex !== null) {
        handleEditTag(trimmed);
      } else {
        handleAddTag(trimmed);
      }
    } else if (e.key === 'Escape') {
      resetEditing();
    }
  }, [inputValue, editingIndex, isLoading, handleAddTag, handleEditTag, resetEditing]);

  const handleChange = useCallback((e) => {
    setInputValue(e.target.value);
  }, []);

  const handleEdit = useCallback((index) => {
    setEditingIndex(index);
    setInputValue(tags[index].name);
  }, [tags]);

  const handleRemove = useCallback((index) => {
    const tagToRemove = tags[index];
    
    if (tagToRemove.subCompanyId && tagToRemove.employeeCount > 0) {
      toastr.error("The sub Company cannot be deleted because it is already in use", '', toastOptions());
      return;
    }
    
    const updatedTags = [...tags];
    
    if (updatedTags[index].actionType === 'delete') {
      updatedTags[index] = {
        ...updatedTags[index],
        actionType: updatedTags[index].subCompanyId ? 'none' : 'insert'
      };
    } else {
      updatedTags[index] = {
        ...updatedTags[index],
        actionType: 'delete'
      };
    }
    
    setTags(updatedTags);
    if (onTagsChange) {
      onTagsChange(updatedTags);
    }

    if (editingIndex === index) {
      resetEditing();
    }
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
        onClick={(e) => {
          e.preventDefault();
          e.stopPropagation();
          handleEdit(index);
        }} 
        className={`tag-text ${editingIndex === index ? 'editing' : ''}`}
      >
        {editingIndex === index ? (
          <input
            ref={inputRef}
            id={`tag-edit-${index}`}
            name={`tag-edit-${index}`}
            value={inputValue}
            onChange={handleChange}
            onKeyDown={handleKeyDown}
            onBlur={handleBlur}
            className="tag-edit-input"
            disabled={isLoading}
            autoComplete="off"
          />
        ) : (
          tag.name
        )}
      </span>
      <button
        type="button"
        onClick={(e) => {
          e.preventDefault();
          e.stopPropagation();
          handleRemove(index);
        }}
        disabled={isLoading}
        className="tag-remove-btn"
      >
        <span className="tag-remove-btn-text">✕</span>
      </button>
    </div>
  ), [editingIndex, inputValue, isLoading, handleEdit, handleChange, handleKeyDown, handleBlur, handleRemove]);

  return (
    <div className="sub-company-container">
      <div className="tag-input-container">
        {tags
          .map((tag, originalIndex) => ({ tag, originalIndex }))
          .filter(({ tag }) => tag.actionType !== 'delete')
          .map(({ tag, originalIndex }) => renderTag(tag, originalIndex))}
        {editingIndex === null && (
          <div className="input-field-wrapper">
            <input
              ref={inputRef}
              id="sub-company-input"
              name="sub-company-input"
              value={inputValue}
              onChange={handleChange}
              onKeyDown={handleKeyDown}
              placeholder="Enter sub company name and press Enter"
              disabled={isLoading}
              className="tag-input-field"
              autoComplete="off"
            />
          </div>
        )}
      </div>
    </div>
  );
};

export default SubCompany;