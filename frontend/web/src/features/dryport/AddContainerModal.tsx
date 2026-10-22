import React, { useState } from 'react';
import './AddContainerModal.css';

interface AddContainerModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (containerData: any) => void;
}

export const AddContainerModal: React.FC<AddContainerModalProps> = ({ isOpen, onClose, onSubmit }) => {
  const [formData, setFormData] = useState({
    containerNumber: '',
    size: '20',
    type: 'Dry',
    demurrageRuleId: 'RULE-001'
  });

  if (!isOpen) return null;

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault(); 
    
    onSubmit(formData);
    
    setFormData({ containerNumber: '', size: '20', type: 'Dry', demurrageRuleId: 'RULE-001' });
    onClose();
  };

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={e => e.stopPropagation()}>
        <div className="modal-header">
          <h2>Diiwaangeli Kunteenar Cusub</h2>
          <button className="close-button" onClick={onClose}>&times;</button>
        </div>

        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="containerNumber">Numbarka Kunteenarka</label>
            <input
              type="text"
              id="containerNumber"
              name="containerNumber"
              className="form-control"
              placeholder="Tusaale: MSCU1234567"
              value={formData.containerNumber}
              onChange={handleChange}
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="size">Cabbirka (Size)</label>
            <select
              id="size"
              name="size"
              className="form-control"
              value={formData.size}
              onChange={handleChange}
            >
              <option value="20">20 ft (Cag)</option>
              <option value="40">40 ft (Cag)</option>
            </select>
          </div>

          <div className="form-group">
            <label htmlFor="type">Nuuca (Type)</label>
            <select
              id="type"
              name="type"
              className="form-control"
              value={formData.type}
              onChange={handleChange}
            >
              <option value="Dry">Dry (Qallalan)</option>
              <option value="Reefer">Reefer (Qaboojiye)</option>
              <option value="Open Top">Open Top</option>
            </select>
          </div>

          <div className="modal-actions">
            <button type="button" className="btn-cancel" onClick={onClose}>
              Jooji (Cancel)
            </button>
            <button type="submit" className="btn-submit">
              Kaydi (Save)
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};
