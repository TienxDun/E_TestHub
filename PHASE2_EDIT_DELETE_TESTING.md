# 🧪 Phase 2 - Edit & Delete Question Testing Guide

## 📋 Testing Information
- **Date**: November 10, 2025
- **App URL**: http://localhost:5230
- **Test User**: Teacher account (hieuthanh5100@gmail.com)
- **MongoDB API**: http://localhost:3000/api

---

## ✅ Pre-Test Setup

### 1. Ensure both servers are running

**Backend (Node.js + MongoDB):**
```bash
cd d:\DevProject\CNPMNC\LyThuyet\New\e-testhub
node src/server.js
# Should see: "Server listening on port 3000"
```

**Frontend (ASP.NET Core):**
```bash
cd d:\DevProject\CNPMNC\LyThuyet\New\E_TestHub
dotnet run
# Should see: "Now listening on: http://localhost:5230"
```

### 2. Login
1. Navigate to: http://localhost:5230
2. Login with teacher credentials (hieuthanh5100@gmail.com)
3. Navigate to "Ngân hàng câu hỏi"

---

## 🎯 Test Cases

### **TC1: Edit Question - Load Edit Form**

**Prerequisites**: 
- At least 1 question exists in Question Bank
- Logged in as Teacher

**Steps**:
1. Go to Question Bank
2. Click **Edit icon (pencil)** on any question
3. Should redirect to `/Teacher/EditQuestion?id={questionId}`

**Expected Results**:
- ✅ Edit form loads successfully
- ✅ All fields are **pre-filled** with existing data:
  - Môn học dropdown shows correct subject
  - Nội dung câu hỏi shows question content
  - Loại câu hỏi shows correct type (Multiple Choice/Essay/True-False)
  - Options A-D show existing answers (for Multiple Choice)
  - Đáp án đúng shows correct answer
  - Điểm shows current score
  - Độ khó shows current difficulty
- ✅ Form is editable
- ✅ No console errors

**Verify in Backend Logs**:
```
GET /api/questions/{id} 200 ... ms
GET /api/subjects 200 ... ms
```

---

### **TC2: Edit Question - Update Multiple Choice Question**

**Prerequisites**: 
- Edit form loaded with Multiple Choice question

**Steps**:
1. Modify **Nội dung câu hỏi**: "Updated question content"
2. Change **Option B**: "New Option B"
3. Change **Đáp án đúng** to "C"
4. Change **Điểm** to 3.0
5. Change **Độ khó** to "Khó (Hard)"
6. Click **"Cập nhật câu hỏi"** button

**Expected Results**:
- ✅ Redirect to Question Bank
- ✅ Green success message: **"Câu hỏi đã được cập nhật thành công!"**
- ✅ Updated question appears in list with new values
- ✅ Statistics update if difficulty changed

**Verify in Backend Logs**:
```
PUT /api/questions/{id} 200 ... ms
GET /api/questions 200 ... ms
```

**Verify in MongoDB**:
```javascript
db.questions.findOne({_id: ObjectId("...")})
// Should show updated content, options, correctAnswer, score, difficultyLevel
```

---

### **TC3: Edit Question - Change Question Type**

**Prerequisites**: 
- Edit form loaded

**Steps**:
1. Change **Loại câu hỏi** from "Trắc nghiệm" to "Tự luận (Essay)"
2. Observe form changes
3. Enter content
4. Click **"Cập nhật câu hỏi"**

**Expected Results**:
- ✅ Options section **hides** when type changes to Essay
- ✅ Correct Answer dropdown **hides**
- ✅ Question updates successfully
- ✅ Type displayed correctly in Question Bank

**Verify**:
- Question Bank shows "Loại: Tự luận"

---

### **TC4: Edit Question - Validation Errors**

**Prerequisites**: 
- Edit form loaded with Multiple Choice question

**Test 4a: Empty Content**
1. Clear **Nội dung câu hỏi** field
2. Click "Cập nhật câu hỏi"

**Expected**:
- ✅ Form shows error: "Nội dung câu hỏi là bắt buộc"
- ✅ Question NOT updated in database

**Test 4b: Missing Correct Answer**
1. Set **Đáp án đúng** to "-- Chọn đáp án đúng --"
2. Click "Cập nhật câu hỏi"

**Expected**:
- ✅ Form shows error: "Vui lòng chọn đáp án đúng"
- ✅ Question NOT updated

**Test 4c: Invalid Score**
1. Set **Điểm** to 0.3 (less than 0.5)
2. Click "Cập nhật câu hỏi"

**Expected**:
- ✅ Form shows error: "Điểm phải từ 0.5 đến 100"
- ✅ Question NOT updated

---

### **TC5: Edit Question - Change Subject**

**Prerequisites**: 
- Multiple subjects exist in database
- Edit form loaded

**Steps**:
1. Change **Môn học** dropdown to different subject
2. Click "Cập nhật câu hỏi"

**Expected Results**:
- ✅ Question updates successfully
- ✅ Question Bank shows new subject name
- ✅ Old subject filter no longer shows this question
- ✅ New subject filter shows this question

**Verify**:
- Filter by old subject → Question NOT in list
- Filter by new subject → Question IN list

---

### **TC6: Delete Question - Single Delete**

**Prerequisites**: 
- At least 2 questions exist in Question Bank
- Logged in as Teacher

**Steps**:
1. Go to Question Bank
2. Note current total count (e.g., 5 questions)
3. Click **Delete icon (trash)** on any question
4. Confirm deletion in popup dialog

**Expected Results**:
- ✅ Confirmation dialog appears: "Bạn có chắc chắn muốn xóa câu hỏi này?"
- ✅ After confirm, question row **fades out** and disappears
- ✅ Page **reloads** automatically
- ✅ Total count decreases by 1 (e.g., 5 → 4)
- ✅ Statistics update correctly
- ✅ Question no longer in database

**Verify in Backend Logs**:
```
DELETE /api/questions/{id} 200 ... ms
GET /api/questions 200 ... ms
```

**Verify in MongoDB**:
```javascript
db.questions.findOne({_id: ObjectId("...")})
// Should return null (deleted)
```

---

### **TC7: Delete Question - Cancel Deletion**

**Steps**:
1. Click Delete icon on a question
2. Click **"Cancel"** in confirmation dialog

**Expected Results**:
- ✅ Dialog closes
- ✅ Question remains in list
- ✅ No API request sent
- ✅ Question still in database

**Verify**:
- No DELETE request in Network tab
- Question count unchanged

---

### **TC8: Delete Question - Delete All Questions**

**Prerequisites**: 
- Exactly 1 question exists

**Steps**:
1. Delete the last question
2. Confirm deletion

**Expected Results**:
- ✅ Question deleted successfully
- ✅ Page shows **empty state**:
  - Icon: Empty box
  - Message: "Chưa có câu hỏi nào"
  - Button: "Nhấn nút 'Thêm câu hỏi' để bắt đầu tạo câu hỏi cho môn học này"
- ✅ Statistics show:
  - Total questions: 0
  - Easy questions: 0
  - Hard questions: 0

---

### **TC9: Edit Question - Handle Invalid ID**

**Steps**:
1. Navigate to: http://localhost:5230/Teacher/EditQuestion?id=invalid123
2. Or: http://localhost:5230/Teacher/EditQuestion?id=000000000000000000000000

**Expected Results**:
- ✅ Redirect to Question Bank
- ✅ Red error message: "Không tìm thấy câu hỏi."
- ✅ No crash or exception

**Verify in Backend Logs**:
```
GET /api/questions/invalid123 404 ... ms
```

---

### **TC10: Delete Question - AJAX Error Handling**

**Prerequisites**: 
- MongoDB API server is stopped

**Steps**:
1. Stop backend server (Ctrl+C in terminal)
2. Try to delete a question
3. Confirm deletion

**Expected Results**:
- ✅ Alert shows: "Có lỗi xảy ra khi xóa câu hỏi."
- ✅ Question remains in list
- ✅ Page does NOT reload
- ✅ No console errors

**Verify**:
- Network tab shows DELETE request failed (ERR_CONNECTION_REFUSED or 500)

---

## 📊 Technical Verification

### **TV1: Check API Request Format**

**Edit Question - PUT Request:**
```json
PUT /api/questions/{id}
Headers:
  Authorization: Bearer {token}
  Content-Type: application/json

Body:
{
  "subjectId": "68f447b83248518ffb9dfa2d",
  "examId": null,
  "content": "Updated question text",
  "type": "multiple-choice",  // ✅ Correct format (with dash)
  "options": ["A", "B", "C", "D"],
  "correctAnswer": "C",
  "score": 3.0,
  "difficultyLevel": "hard"  // ✅ Lowercase
}
```

**Delete Question - DELETE Request:**
```json
DELETE /api/questions/{id}
Headers:
  Authorization: Bearer {token}
```

---

### **TV2: Check MongoDB Data**

**After Edit:**
```javascript
db.questions.findOne({_id: ObjectId("...")})
```

Expected fields updated:
- `content`: New text
- `options`: Updated array
- `correctAnswer`: New answer
- `score`: New score
- `difficultyLevel`: "easy", "medium", or "hard"
- `type`: "multiple-choice", "essay", or "true-false"
- `updatedAt`: New timestamp (if field exists)

**After Delete:**
```javascript
db.questions.findOne({_id: ObjectId("...")})
// Returns: null
```

---

### **TV3: Check Response Parsing**

**Edit Question Response:**
- C# should handle 200 OK with any response body
- `UpdateQuestionAsync()` returns `bool success`
- Does NOT need to parse response object

**Delete Question Response:**
- Returns JSON: `{"deleted": true}` or `{"success": true}`
- C# parses to bool: `DeleteQuestionAsync()` returns `true`/`false`

---

## 🐛 Known Issues & Limitations

### **Issue 1: Edit Form Doesn't Preserve ExamId**
- **Description**: If question belongs to an exam, ExamId might be lost on update
- **Workaround**: Don't edit questions that are already assigned to published exams
- **Fix**: Add ExamId to EditQuestionViewModel

### **Issue 2: Delete Confirmation in Vietnamese**
- **Description**: Browser default confirm() shows English buttons
- **Workaround**: Use custom modal with Vietnamese text
- **Enhancement**: Implement Bootstrap modal for better UX

### **Issue 3: No Undo Delete**
- **Description**: Deleted questions cannot be recovered
- **Workaround**: Implement soft delete (isDeleted flag)
- **Enhancement**: Add trash/restore feature

---

## 📋 Test Results Summary

| Test Case | Status | Notes |
|-----------|--------|-------|
| TC1: Load Edit Form | ⬜ Not Tested | |
| TC2: Update Multiple Choice | ⬜ Not Tested | |
| TC3: Change Question Type | ⬜ Not Tested | |
| TC4: Validation Errors | ⬜ Not Tested | |
| TC5: Change Subject | ⬜ Not Tested | |
| TC6: Single Delete | ⬜ Not Tested | |
| TC7: Cancel Delete | ⬜ Not Tested | |
| TC8: Delete All | ⬜ Not Tested | |
| TC9: Invalid ID | ⬜ Not Tested | |
| TC10: AJAX Error | ⬜ Not Tested | |

**Legend:**
- ✅ Pass
- ❌ Fail
- ⚠️ Partial
- ⬜ Not Tested

---

## 🎯 Quick Test Checklist

**Edit Question:**
- [ ] Form loads with correct data
- [ ] Can update all fields
- [ ] Validation works
- [ ] Success message appears
- [ ] Changes saved in database

**Delete Question:**
- [ ] Confirmation dialog appears
- [ ] Can cancel deletion
- [ ] Question removed from list
- [ ] Statistics update
- [ ] Deleted from database

---

## 📝 Testing Notes

Add your testing observations here:

```
Date: _______
Tester: _______

Test Results:
- Edit Question: _______
- Delete Question: _______

Issues Found:
1. _______
2. _______

Comments:
_______
```

---

## ✅ Sign-off

- [ ] All test cases executed
- [ ] All critical bugs fixed
- [ ] Documentation complete
- [ ] Ready for Exam Management phase

**Tested by**: _______  
**Date**: _______  
**Approved by**: _______
